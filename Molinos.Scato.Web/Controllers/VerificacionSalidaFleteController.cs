using System;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadVerificacioneSalidaFlete)]
    public class VerificacionSalidaFleteController : BaseController
    {
        private readonly IServicioActividadFactory<IVerificacionSalidaFleteService> factory;
        private readonly ILogger log;

        public VerificacionSalidaFleteController(ILogger log, IServicioActividadFactory<IVerificacionSalidaFleteService> factory, IServicioRepositorio servicio)
            : base(servicio)
        {
            this.factory = factory;
            this.log = log;
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerDatosDeInstanciaPorGuid(id);

            ViewBag.WorkflowId = id;
            ViewBag.WorkflowCodigo = recorrido.WorkflowCodigo;
            ViewBag.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;

            var control = servicio.ObtenerControlRecorrido(id, "VerificacionSalidaFlete");
            ViewBag.Mensaje = control != null ? control.Comentario : Textos.VerificacionSalidaFlete_Error;

            return View();
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult Reintentar(int workflowDefinicionId, string workflowCodigo, Guid workflowId, DatosUsuario datosUsuario)
        {
            var controlRecorrido = new ControlRecorridoDto {Actividad = Textos.ActVerificacionSalidaFlete, ActividadXaml = "VerificacionSalidaFlete", WorkflowInstanceId = workflowId, PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId, NombreUsuario = datosUsuario.NombreUsuario};

            var serviciowf = factory.CrearServicio(workflowDefinicionId);
            serviciowf.VerificacionSalidaFlete(workflowId, true, controlRecorrido);
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult Cancelar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        public JsonResult Test()
        {
            var servicioCompliance = new ChannelFactory<DatosPort>("DatosPortV2").CreateChannel();
            var target = new VerificarSalidaFlete();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servicio);
            host.Extensions.Add(servicioCompliance);
            host.Extensions.Add(() => new ScatoPersistenceParticipant() { CentroId = 5});

            host.InArguments.Patente = "LTI578";
            host.InArguments.ChoferId = 36065;
            host.InArguments.TransportistaId = 2658;
            
            var retorno = host.TestActivity();
            return Json(new { Msg = (string)retorno.First(f => f.Key == "MensajeError").Value },JsonRequestBehavior.AllowGet);
        }
    }
}
