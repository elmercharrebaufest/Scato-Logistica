using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadEnTransito)]
    public class EnTransitoController : BaseController
    {
        private readonly IServicioActividadFactory<IEjecutarService> factory;
        private readonly ILogger log;

        public EnTransitoController(ILogger log, IServicioActividadFactory<IEjecutarService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }
        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.Workflow = recorrido.Workflow.Codigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            var observacion = new ObservacionRDto { WorkflowInstanceId = id };

            return View(observacion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(ObservacionRDto observacion, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            log.Debug($"Actividad en Transito para workflow { workflow } identificado { observacion.WorkflowInstanceId }.");
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActEnTransito,
                ActividadXaml = "EnTransito",
                WorkflowInstanceId = observacion.WorkflowInstanceId,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario,
                Decision = true,
                Comentario = observacion.Observaciones
            };

            var service = factory.CrearServicio(workflowDefinicionId);
            service.Ejecutar(observacion.WorkflowInstanceId, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }
    }
}
