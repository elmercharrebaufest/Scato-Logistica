using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ReiniciarIntercomunicador)]
    public class IntercomunicadorController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;

        public IntercomunicadorController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.log = log;
        }

        public ActionResult Index()
        {
            ViewBag.Servidor = configuracion.AppSettings["ICWebServerUrl"];
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Reiniciar(DatosUsuario datosUsuario)
        {
            var servidor = configuracion.AppSettings["ICWebServerUrl"];
            var resultado = servicioComandos.Ejecutar(new ReiniciarIntercomunicador
            {
                NombreUsuario = datosUsuario.NombreUsuario,
                Servidor = servidor
            });

            if (resultado.HayErrores)
            {
                var mensajeError = string.Join("; ", resultado.Errores.Values);
                log.Error("Error al reiniciar el servicio Intercomunicador: {0}", mensajeError);
                return Json(new { exito = false, mensaje = mensajeError });
            }

            return Json(new { exito = true, mensaje = "El servicio Intercomunicador fue reiniciado correctamente." });
        }
    }
}
