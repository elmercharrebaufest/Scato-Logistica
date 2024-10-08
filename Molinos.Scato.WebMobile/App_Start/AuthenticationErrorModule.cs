using System;
using System.Web;

namespace Molinos.Scato.WebMobile.App_Start
{
    public class AuthenticationErrorModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.Error += OnError;
        }

        private void OnError(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if(app == null)
            {
                return;
            }

            Exception exception = app.Server.GetLastError();

            if (!typeof(InvalidOperationException).IsInstanceOfType(exception) ||
                exception.Source != "System.IdentityModel.Services" ||
                !exception.Message.StartsWith("ID1059"))
            {
                return;
            }

            app.Response.Redirect("~/ErrorPages/ErrorEnCertificado.html");
        }

        public void Dispose() { }
    }
}