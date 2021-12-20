using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    [Autorizacion(PermisosScato.ActividadAutorizarTiempoEnTransito)]
    public class AutorizarTiempoEnTransitoController : BaseController
    {
        private readonly IServicioActividadFactory<IAutorizarTiempoEnTransitoService> factory;
        private ILogger log;

        public AutorizarTiempoEnTransitoController(ILogger log, IServicioActividadFactory<IAutorizarTiempoEnTransitoService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);
            var control = servicio.ObtenerAutorizacionTiempoEnTransito(id, recorrido.DatosProximaActividad,datosUsuario.NombreUsuario);
            
            ViewBag.Workflow = recorrido.WorkflowCodigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Tiempo = control.TiempoEnTransito;
            ViewBag.TiempoAceptado = control.TiempoAceptado;

            return View(control);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(AutorizacionTiempoEnTransitoDto autorizacionTiempo, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = autorizacionTiempo.WorkflowInstanceId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActAutorizarTiempoEnTransito,
                ActividadXaml = "AutorizarTiempoEnTransito",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                Decision = autorizacionTiempo.Decision,
                Comentario = autorizacionTiempo.Comentario,
                Mensaje = autorizacionTiempo.Decision ? null : "Exceso Tiempo Tránsito"
            };
            var controlTiempoAceptadoService = factory.CrearServicio(workflowDefinicionId);
            var resultado = controlTiempoAceptadoService.AutorizarTiempoEnTransito(autorizacionTiempo, autorizacionTiempo.WorkflowInstanceId, controlRecorrido, autorizacionTiempo.Decision);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = autorizacionTiempo.WorkflowInstanceId });
        }
    }
}
