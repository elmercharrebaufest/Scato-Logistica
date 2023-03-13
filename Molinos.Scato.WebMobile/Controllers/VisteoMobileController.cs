using System;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Helpers;
using Molinos.Scato.WebMobile.Helpers.Molinos.Scato.Dominio.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class VisteoMobileController : Controller
    {
        private readonly IServicioActividadFactory<IVisteoService> factory;
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;


        public VisteoMobileController(ILogger log, IServicioActividadFactory<IVisteoService> factory, IServicioRepositorio servicio)
        {
            this.factory = factory;
            this.log = log;
            this.servicio = servicio;
        }


        [HttpPost]
        public ActionResult Index(Guid instanciaWorflow, int workflowDefinicionId, bool esValido, string actividad, string actividadXaml, string mensaje, string comentario)
        {
            var service = factory.CrearServicio(workflowDefinicionId);

            var response = new RespuestaEstandarDto();

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanciaWorflow,
                NombreUsuario = ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value,
                Fecha = DateTime.Today,
                Actividad = string.IsNullOrWhiteSpace(actividad) ? Textos.ActVisteo : actividad,
                ActividadXaml = string.IsNullOrWhiteSpace(actividadXaml) ? Textos.Visteo : actividadXaml,
                Decision = esValido,
                Mensaje = string.IsNullOrWhiteSpace(mensaje) ? "" : mensaje,
                Comentario = string.IsNullOrWhiteSpace(comentario) ? "" : comentario,

            };

            var resultado = service.Visteo(controlRecorrido, controlRecorrido.WorkflowInstanceId);

            if (resultado.HayErrores)
            {
                foreach (var item in resultado.Errores)
                {
                    response.Mensajes.Add(new MensajeEstandarDto { Mensaje = item.Value, TipoDeMensaje = TipoDeMensajeDeRespuesta.Error });
                }
            }
            return Json(response);
        }

        public ActionResult RechazarNoGranos(Guid instanciaWorflow, int workflowDefinicionId)
        {
            log.Info("{0} - Visteo Rechazar", workflowDefinicionId);
            ViewBag.Motivos = servicio.ListarMotivos().ToSelectList(x => x.Descripcion, x => x.Descripcion);
            var actividad = servicio.ObtenerEntidadActividadPorCodigos(Constantes.Entidad.Visteo, Constantes.TipoDeActividad.Rechazar);
            ViewBag.Workflow = workflowDefinicionId;
            ViewBag.WorkflowDefinicionId = workflowDefinicionId;
            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanciaWorflow,
                NombreUsuario = ClaimsPrincipal.Current.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value,
                Actividad = actividad.Entidad.Descripcion + "/" + actividad.TipoDeActividad.Descripcion,
                ActividadXaml = actividad.Entidad.Descripcion,
            };
            return View("_VehiculoRechazado", controlRecorrido);
        }

    }
}
