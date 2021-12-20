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
    [Autorizacion(PermisosScato.ActividadIngresoDeObservaciones)]
    public class IngresoDeObservacionesController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresoDeObservacionesService> factory;
        private ILogger log;

        public IngresoDeObservacionesController(ILogger log, IServicioActividadFactory<IIngresoDeObservacionesService> factory, IServicioRepositorio servicio)
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
            var observacion = servicio.ObtenerObservacion(id)
                ?? new ObservacionDto{WorkflowInstanceId = id};

            return View(observacion);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(ObservacionDto observacion, string workflow, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActIngresoDeObservaciones,
                    ActividadXaml = "IngresoDeObservaciones",
                    WorkflowInstanceId = observacion.WorkflowInstanceId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

            var ingresoDeObservacionesService = factory.CrearServicio(workflowDefinicionId);
            Resultado resultado = ingresoDeObservacionesService.IngresoDeObservaciones(observacion, observacion.WorkflowInstanceId, controlRecorrido);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = observacion.WorkflowInstanceId });
        }
    }
}
