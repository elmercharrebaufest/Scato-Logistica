using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using Ninject.Extensions.Logging;
using System;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.EstadoVolcadoras)]
    public class EstadoVolcadorasController : Controller
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicio;
        private readonly IServicioComandos servicioComandos;

        public EstadoVolcadorasController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioComandos = servicioComandos;
        }

        // GET: /CartelesVolcables/
        public ActionResult Index()
        {
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas();

            return View(hidraulicas);
        }

        public JsonResult EstadoDeVolcadoras()
        {
            var hidraulicas = servicio.ListarHidraulicasAutomatizadas();
          
            return Json(new {hidraulicas}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CambiarEstado(int nuevoEstado, int id, string nombre)
        {
            try
            {
                servicioComandos.Ejecutar(new ActualizarLlamadoAutomaticoHidraulica { Id = id, Estado = (EstadoHidraulica)nuevoEstado, Patente = string.Empty });
            }
            catch (Exception e)
            {
                log.Error(e, $"No se pudo actualizar la hidráulica {nombre}");
            }
            return Json("ok", JsonRequestBehavior.AllowGet);

        }

    }
}
