using System.Collections.Generic;
using System.Globalization;
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
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmAnalisisObligatorio)]
    public class AnalisisObligatorioController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public AnalisisObligatorioController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
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
            ViewBag.Items = servicio.ListarPaginadoAnalisisObligatorios(filtro, paginacion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).OrderBy(p => p.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(AnalisisObligatorioDto model, string puestosDeTrabajo, DatosUsuario datosUsuario)
        {
            model.Id = 0;
            if (ModelState.IsValid && puestosDeTrabajo != "")
            {
                var listapuestos = puestosDeTrabajo.FromJson<List<PuestoDeTrabajoDto>>();
                model.CentroId = datosUsuario.CentroId;
                model.PuestosDeTrabajoAsociados = listapuestos;

                var resultado = servicioComandos.Ejecutar(new CrearAnalisisObligatorio { Dto = model , Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            if (puestosDeTrabajo == "")
            {
                ModelState.AddModelError("PuestosDeTrabajoAsociados", string.Format(Textos.Error_Requerido, "Puesto de Trabajo"));
            }

            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).OrderBy(p => p.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var aModificar = servicio.ObtenerAnalisisObligatorio(id);
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).OrderBy(p => p.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
            return View(aModificar);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(AnalisisObligatorioDto model, string puestosDeTrabajo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid && puestosDeTrabajo != "")
            {
                var listapuestos = puestosDeTrabajo.FromJson<List<PuestoDeTrabajoDto>>();
                model.CentroId = datosUsuario.CentroId;
                model.PuestosDeTrabajoAsociados = listapuestos;

                var resultado = servicioComandos.Ejecutar(new ModificarAnalisisObligatorio { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
            }
            if (puestosDeTrabajo == "")
            {
                ModelState.AddModelError("PuestosDeTrabajoAsociados", string.Format(Textos.Error_Requerido, "Puesto de Trabajo"));
            }

            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).OrderBy(p => p.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
            return View(model);
        }
        public JsonResult ObtenerPuestos(int id)
        {
            var analisis = servicio.ObtenerAnalisisObligatorio(id);
            var puestos = analisis != null ? analisis.PuestosDeTrabajoAsociados.ToList() : null;
            return Json(puestos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarAnalisisObligatorio { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }
    }
}
