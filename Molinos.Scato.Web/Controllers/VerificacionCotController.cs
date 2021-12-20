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
    [Autorizacion(PermisosScato.VerificacionCot)]
    public class VerificacionCotController : BaseController
    {
        private readonly IServicioActividadFactory<IVerificacionCotService> factory;
        private ILogger log;

        public VerificacionCotController(ILogger log, IServicioActividadFactory<IVerificacionCotService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        [DatosUsuario]
        public ActionResult Index(Guid id, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);

            ViewBag.TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso;
            ViewBag.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            ViewBag.Patente = recorrido.Patente;
            ViewBag.WorkflowInstanceId = id;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            ViewBag.Workflow = recorrido.Workflow.Codigo;

            var control = servicio.ObtenerControlRecorrido(id, "VerificacionCot");
            if (control != null)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("WorkflowId", control.Comentario);
                ModelState.AgregarErrores(resultado);
            }

            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(string workflow, int workflowDefinicionId, Guid workflowInstanceId, DatosUsuario datosUsuario, bool esManual, string numeroCot)
        {
            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActVerificacionCot,
                    ActividadXaml = "VerificacionCot",
                    WorkflowInstanceId = workflowInstanceId,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

            var verificacionCotService = factory.CrearServicio(workflowDefinicionId);
            var resultado = verificacionCotService.VerificacionCot(workflowInstanceId, controlRecorrido, esManual, numeroCot);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index");
        }
    }
}
