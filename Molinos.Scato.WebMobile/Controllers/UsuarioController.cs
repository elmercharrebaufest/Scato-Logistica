using System;
using System.Configuration;
using System.IdentityModel.Services;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.Controllers
{
    public class UsuarioController : Controller
    {
        [AllowAnonymous]
        public void SignOut()
        {
            var adfsLogoffUrl = ConfigurationManager.AppSettings["UrlAdfsLogoff"];
            var authModule = FederatedAuthentication.WSFederationAuthenticationModule;
            var signoutURL = WSFederationAuthenticationModule.GetFederationPassiveSignOutUrl(authModule.Issuer, adfsLogoffUrl, null);
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(signoutURL), new Uri(authModule.Realm));
        }
    }
}