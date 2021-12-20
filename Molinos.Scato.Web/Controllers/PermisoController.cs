using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmPermiso)]
    public class PermisoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public PermisoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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
            ViewBag.Items = servicio.ListarPaginadoPermisos(filtro, paginacion);
        }

        public ActionResult Crear()
        {
            SetearCodigos(null);

            return View();
        }

        [HttpPost]
        public ActionResult Crear(PermisoDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearPermiso { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCodigos(null);
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerPermiso(id);

            SetearCodigos(aModificar.Codigo);

            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(PermisoDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarPermiso { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearCodigos(model.Codigo);
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarPermiso { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void SetearCodigos(PermisosScato? codigoActual)
        {
            List<PermisosScato> values = ((PermisosScato[])Enum.GetValues(typeof(PermisosScato))).ToList();
            var permisos = servicio.ListarPermisos();

            foreach (var permisoDto in permisos)
            {
                if (values.Any(v => v == permisoDto.Codigo.GetValueOrDefault() && v != codigoActual.GetValueOrDefault()))
                {
                    var existente = values.Single(v => permisoDto.Codigo != null && v == permisoDto.Codigo.Value);
                    values.Remove(existente);
                }
            }

            var actividades = new List<SelectListItem>
                {
                    new SelectListItem {Selected = true, Value = "", Text = Textos.DefaultActividad}
                };

            actividades.AddRange(Enum.GetValues(typeof(Dominio.Enums.Actividades)).Cast<Dominio.Enums.Actividades>().ToSelectList(x => x.ToString(), x => x.DisplayEnum()));


            ViewBag.Codigos = values.OrderBy(x => x.ToString()).ToSelectList(x => x.ToString(), x => x.DisplayEnum());
            ViewBag.Actividades = actividades;
        }
    }
}
