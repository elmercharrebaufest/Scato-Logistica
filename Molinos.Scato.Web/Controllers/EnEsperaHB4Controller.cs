using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.EnEsperaHB4)]
    public class EnEsperaHB4Controller : BaseController
    {
        private readonly IServicioActividadFactory<IEnEsperaHB4Service> factory;
        private ILogger log;

        public EnEsperaHB4Controller(ILogger log, IServicioActividadFactory<IEnEsperaHB4Service> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            ViewBag.PermiteRechazo = recorrido.Workflow.Codigo != Constantes.WorkFlow.workflowRedespacho;
            return View(recorrido);
        }

        [DatosUsuario]
        [HttpPost]
        public JsonResult Index(Guid workflowInstance, int workflowDefinicionId, DatosUsuario datosUsuario)
        {
            var response = new RespuestaEstandarDto();
            try
            {
                var controlRecorrido = new ControlRecorridoDto
                {
                    WorkflowInstanceId = workflowInstance,
                    NombreUsuario = datosUsuario.NombreUsuario,
                    Actividad = Textos.ActEnEsperaHB4,
                    ActividadXaml = "EnEsperaHB4",
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
                };

                var service = factory.CrearServicio(workflowDefinicionId);

                var resultado = service.EnEsperaHB4(controlRecorrido, workflowInstance, false);
                if (resultado.HayErrores)
                {
                    foreach (var item in resultado.Errores)
                    {
                        response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error al avanzar de etapa Pendiente Analisis HB4 para el workflow {workflowInstance}";
                log.Error(ex, errorMessage);
                response.Mensajes.Add(new MensajeEstandarDto { Mensaje = errorMessage, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
            }
            return Json(response);
        }

        [DatosUsuario]
        public ActionResult Rechazar(Guid workflowInstance, int workflowDefinicionId ,DatosUsuario datosUsuario)
        {
            log.Info("{0} - EnEsperaHB4 Rechazar", workflowInstance);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = workflowInstance,
                NombreUsuario = datosUsuario.NombreUsuario,
                Actividad = Textos.ActEnEsperaHB4 + "/" + Textos.Rechazar,
                ActividadXaml = "EnEsperaHB4",
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId
            };

            return View("_TransportistaRechazado", controlRecorrido);
        }

        [DatosUsuario]
        public ActionResult TransportistaRechazado(int workflowDefinicionId, ControlRecorridoDto controlRecorrido)
        {
            log.Info("{0} - EnEsperaHB4 rechazado", workflowDefinicionId);
            controlRecorrido.Decision = true;
            var service = factory.CrearServicio(workflowDefinicionId);
            var resultado = service.EnEsperaHB4(controlRecorrido, controlRecorrido.WorkflowInstanceId, true);
            if (!resultado.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            ModelState.AgregarErrores(resultado);
            return RedirectToAction("Index", new { id = controlRecorrido.WorkflowInstanceId });
        }
    }
}