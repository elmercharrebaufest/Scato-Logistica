using System;
using System.Web.Mvc;
using System.Web.UI;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebOperaciones.Atributos;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebOperaciones.Controllers
{
    [Autorizacion(PermisosScato.ScatoPuerto)]
    public class IndexController : ConsultasController
    {
        private readonly IFirmaProvider firmaProvider;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicio;
        private readonly ILogger log;

        public IndexController(
            ILogger log,
            IServicioRepositorio servicio,
            IFirmaProvider firmaProvider, 
            IConfiguracionProvider configuracion,
            IServicioComandos servicioComandos
            ) : base(log, servicio, configuracion)
        {
            this.log = log;
            this.servicio = servicio;
            this.firmaProvider = firmaProvider;
            this.configuracion = configuracion;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return PartialView("_Menu");
        }
        
        [OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
        public FileContentResult Logo()
        {
            return File(firmaProvider.ObtenerLogo(), "image/png");
        }

        [AllowAnonymous]
        public string Favicon()
        {
            return "data:image/x-icon;base64," + Convert.ToBase64String(firmaProvider.ObtenerFavicon());
        }
    }
}
