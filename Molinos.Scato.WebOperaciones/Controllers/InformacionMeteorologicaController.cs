
using System.Security.Claims;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.WebOperaciones.Atributos;
using Molinos.Scato.WebOperaciones.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class InformacionMeteorologicaController : Controller
    {
        //
        // GET: /InformacionMeteorologica/
        private readonly IServicioRepositorio servicio;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly ILogger log;

        public InformacionMeteorologicaController(ILogger log, IServicioRepositorio servicio, IServicioOrquestador servicioOrquestador)
        {
            this.log = log;
            this.servicio = servicio;
            this.servicioOrquestador = servicioOrquestador;
        }

        public ActionResult Index()
        {
            var estacion = ClaimsPrincipal.Current.GetUserClaim("EstacionMeteorologica");
            var res = servicioOrquestador.Ejecutar(new EjecutarEstacionMeteorologica { CodigoDispositivo = estacion.Value });
            ViewBag.datos = (ResultadoMeteorologica)res;
            return View();
        }

    }
}
