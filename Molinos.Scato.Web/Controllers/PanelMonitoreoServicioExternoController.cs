using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    public class PanelMonitoreoServicioExternoController : Controller
    {
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IServicioLlamadoAutomatico servicioLlamadoAutomatico;
        private readonly ILogger log;

        public PanelMonitoreoServicioExternoController(ILogger log, IServicioRepositorio servicioRepositorio, IServicioLlamadoAutomatico servicioLlamadoAutomatico)
        {
            this.log = log;
            this.servicioRepositorio = servicioRepositorio;
            this.servicioLlamadoAutomatico = servicioLlamadoAutomatico;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EstadoServiciosPartial()
        {
            var model = servicioRepositorio.ListarMonitoreoServicioExterno();
            return PartialView("_EstadoServiciosExternos", model);
        }

        [HttpPost]
        public ActionResult ActualizarTodosServiciosExternos()
        {
            try
            {
                var model = servicioRepositorio.ListarMonitoreoServicioExterno();

                foreach (var servicioExterno in model)
                    servicioLlamadoAutomatico.EjecutarHealthCheckAsync(servicioExterno.KeyJob);
            }
            catch (Exception ex)
            {
                log.Error("Error al actualizar todos los servicios externos", ex);
            }

            return Json(new { success = true, message = "Healthchecks iniciados" });
        }

        [HttpPost]
        public ActionResult ActualizarServicioExterno(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Json(new { success = false, message = "Key invalida" });

            try
            {
                servicioLlamadoAutomatico.EjecutarHealthCheckAsync(key);
            }
            catch (Exception ex)
            {
                log.Error($"Error al actualizar el servicio externo {key}", ex);
            }
            return Json(new { success = true, message = "Healthcheck iniciado" });
        }
    }
}