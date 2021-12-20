using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmRol)]
    public class RolController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public RolController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }


        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoRoles(filtro, paginacion);
            ViewBag.Usuarios = servicio.ListarUsuarios();
        }

        public ActionResult Crear()
        {
            ViewBag.TiposPermiso = Enum.GetValues(typeof(TipoPermiso)).Cast<TipoPermiso>().ToSelectList(x => ((int)x).ToString(), x => x.ToString());
            ViewBag.Permisos = new SelectList(new List<PermisoDto>());
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(RolDto model, string permisos)
        {
            if (permisos == "" || permisos == "[]")
            {
                ModelState.AddModelError("Descripcion", Textos.Rol_RolSinPermisos);
            }

            if (ModelState.IsValid)
            {
                var listaPermisos = permisos.FromJson<List<PermisoDto>>();

                model.PermisosAsociados = listaPermisos;

                var resultado = servicioComandos.Ejecutar(new CrearRol { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }                
            ViewBag.TiposPermiso = Enum.GetValues(typeof(TipoPermiso)).Cast<TipoPermiso>().ToSelectList(x => ((int)x).ToString(), x => x.ToString());
            ViewBag.Permisos = new SelectList(new List<PermisoDto>());
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerRol(id);
            ViewBag.TiposPermiso = Enum.GetValues(typeof(TipoPermiso)).Cast<TipoPermiso>().ToSelectList(x => ((int)x).ToString(CultureInfo.InvariantCulture), x => x.ToString());
            ViewBag.Permisos = new SelectList(new List<PermisoDto>());
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(RolDto model, string permisos)
        {
            if (permisos == "" || permisos == "[]")
            {
                ModelState.AddModelError("Descripcion", Textos.Rol_RolSinPermisos);
            }

            if (ModelState.IsValid)
            {
                var listaPermisos = permisos.FromJson<List<PermisoDto>>();

                model.PermisosAsociados = listaPermisos;

                var resultado = servicioComandos.Ejecutar(new ModificarRol { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }

            }
            ViewBag.TiposPermiso = Enum.GetValues(typeof(TipoPermiso)).Cast<TipoPermiso>().ToSelectList(x => ((int)x).ToString(CultureInfo.InvariantCulture), x => x.ToString());
            ViewBag.Permisos = new SelectList(new List<PermisoDto>());
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarRol { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public JsonResult ObtenerPermisos(int id)
        {
            var rol = servicio.ObtenerRol(id);
            var permisos = rol != null ? rol.PermisosAsociados.ToList() : new List<PermisoDto>();

            var jsonPermisos = new List<Object>();
            foreach (var permiso in permisos)
            {
                jsonPermisos.Add(new
                        {
                            Id = permiso.Id,
                            TipoPermiso = permiso.TipoPermiso,
                            TipoPermisoDesc = permiso.TipoPermiso.ToString(),
                            Descripcion = permiso.Descripcion
                        });
            }

            return Json(jsonPermisos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiltrarPermisosPorTipo(TipoPermiso tipoPermiso)
        {
            var permisos = servicio.ListarPermisos().Where(p => p.TipoPermiso == tipoPermiso)
                .ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            return Json(permisos, JsonRequestBehavior.AllowGet);
        }
    }
}
