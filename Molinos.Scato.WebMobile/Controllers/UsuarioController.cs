using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class UsuarioController : Controller
    {
        [AllowAnonymous]
        public void SignOut()
        {
            // Sign out from local cookie authentication
            HttpContext.GetOwinContext().Authentication.SignOut(
                CookieAuthenticationDefaults.AuthenticationType,
                OpenIdConnectAuthenticationDefaults.AuthenticationType
            );
        }
    }
}