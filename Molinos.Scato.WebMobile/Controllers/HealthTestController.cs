using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
namespace Molinos.Scato.WebMobile.Controllers
{
    public class HealthTestController : Controller
    {
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;
        private static bool servidorOperando;
        private static readonly object LockObject = new object();

        public HealthTestController(ILogger log, IServicioRepositorio servicioRepositorio, IConfiguracionProvider configuracion)
        {
            this.log = log;
            this.servicioRepositorio = servicioRepositorio;
            this.configuracion = configuracion;
        }


        public ActionResult Index()
        {
            return Content("OK");
        }
    }
}
