using System.Configuration;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class UsuarioController : Controller
    {
        [AllowAnonymous]
        public ActionResult SignOut()
        {
            var adfsLogoutUrl = ConfigurationManager.AppSettings["UrlAdfsLogoff"];
            return Redirect(adfsLogoutUrl);
        }
    }
}