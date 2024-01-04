using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebMobile.Atributos;
using System.Web.Mvc;
using System.Web.UI;

namespace Molinos.Scato.WebMobile.Controllers
{
    [Autorizacion(PermisosScato.WebMobile)]
    public class IndexController : Controller
    {
        private readonly IFirmaProvider firmaProvider;

        public IndexController(
            IFirmaProvider firmaProvider
            )
        {
            this.firmaProvider = firmaProvider;
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
            return "";
        }
    }
}