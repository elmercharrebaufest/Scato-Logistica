using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class TiempoEnTransitoController : Controller
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IServicioActividadFactory<IAutorizarTiempoEnTransitoService> factory;


        public TiempoEnTransitoController(ILogger log, IServicioRepositorio servicioRepositorio, IServicioActividadFactory<IAutorizarTiempoEnTransitoService> factory)
        {
            this.log = log;
            this.servicioRepositorio = servicioRepositorio;
            this.factory = factory;
        }

        public ActionResult Aceptar(Guid id)
        {
            return Ejecutar(id, true);
        }

        public ActionResult Rechazar(Guid id)
        {
            return Ejecutar(id, false);
        }
    
        private ActionResult Ejecutar(Guid id, bool autorizado)
        {
            var datos = servicioRepositorio.ObtenerDatosDeInstanciaPorGuid(id);
            var nombreUsuario = ClaimsPrincipal.Current.GetUserClaim(ClaimTypes.NameIdentifier).Value;
            var control = servicioRepositorio.ObtenerAutorizacionTiempoEnTransito(id, datos.DatosProximaActividad, nombreUsuario);

            var controlRecorrido = new ControlRecorridoDto
            {
                WorkflowInstanceId = id,
                NombreUsuario = nombreUsuario,
                Actividad = Textos.ActAutorizarTiempoEnTransito,
                ActividadXaml = "AutorizarTiempoEnTransito",
                PuestoDeTrabajoId = 0,
                Decision = autorizado,
                Comentario = string.Format(Textos.ExcesoDeTiempo_Detalle, control.TiempoAceptado, control.TiempoEnTransito),
                Mensaje = autorizado ? null : "Exceso Tiempo Tránsito"
            };
            control.Decision = autorizado;
            control.Comentario = string.Format(Textos.ExcesoDeTiempo_Detalle, control.TiempoAceptado, control.TiempoEnTransito);

            try
            {
                var service = factory.CrearServicio(datos.WorkflowDefinicionId);
                var resultado = service.AutorizarTiempoEnTransito(control, id, controlRecorrido, autorizado);
                if (resultado.HayErrores)
                {
                    var motivo = servicioRepositorio.ObtenerEstadoRecorrido(id);
                    ViewBag.Mensaje = "Error: " + resultado.Errores.First().Value;
                    ViewBag.Motivo = motivo.Descripcion;
                }
                else
                {
                    ViewBag.Mensaje = "El camión fue " + (autorizado ? "autorizado correctamente" : "rechazado");
                }
            }
            catch(Exception e)
            {
                var faultException = e as System.ServiceModel.FaultException;
                if (faultException != null && faultException.Code.Name == "OperationNotAvailable")
                {
                    var motivo = servicioRepositorio.ObtenerEstadoRecorrido(id);
                    if(motivo != null && !string.IsNullOrEmpty(motivo.Descripcion))
                    {
                        ViewBag.Motivo = motivo.Descripcion;
                    }
                    ViewBag.Mensaje = "Error: " + String.Format(Textos.Error_ActividadYaEjecutada, Textos.ActAutorizarTiempoEnTransito);
                }
                else
                {
                    ViewBag.Mensaje = "Error: " + e.Message;
                }

            }
            
            return View("Index", control);
        }

    }
}
