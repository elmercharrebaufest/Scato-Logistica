using System;
using System.Globalization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Molinos.Scato.WebMobile.App_Start;
using Molinos.Scato.WebMobile.Filtros;
using log4net;

namespace Molinos.Scato.WebMobile
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Log4NetConfig.Configure(Server);

            var binder = new DateTimeModelBinder(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm");
            ModelBinders.Binders.Add(typeof(DateTime), binder);
            ModelBinders.Binders.Add(typeof(DateTime?), binder);
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            var logger = LogManager.GetLogger(GetType());
            logger.Error("Excepción no manejada: ", ex);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {

        }
    }
}