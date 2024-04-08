using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class UsuarioController : Controller
    {
        [AllowAnonymous]
        public ActionResult SignOut()
        {
            var adfsLogoutUrl = "https://bfdev271.baunet.local/adfs/ls/?wa=wsignout1.0";
            return Redirect(adfsLogoutUrl);
        }
    }
}