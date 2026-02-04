using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    /// <summary>
    /// Controller for handling authentication actions (sign-in, sign-out)
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        /// <summary>
        /// Initiates sign-in with Azure Entra ID
        /// </summary>
        [AllowAnonymous]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                // Use Url.Content to get app-relative URL for the redirect
                var redirectUri = Url.Content("~/");
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                Response.Redirect(Url.Content("~/"));
            }
        }

        /// <summary>
        /// Signs out the current user from both the application and Azure Entra ID
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        /// <summary>
        /// Alternative sign-out method that can be called via GET
        /// Use with caution - POST with anti-forgery token is preferred
        /// </summary>
        [AllowAnonymous]
        public void SignOutDirect()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
