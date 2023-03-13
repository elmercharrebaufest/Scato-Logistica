using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Molinos.Scato.Web.App_Start;
using log4net;
using Microsoft.ApplicationInsights.Extensibility;

namespace Molinos.Scato.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            BrockAllen.CookieTempData.CookieTempDataProvider.ValidationException += CookieTempDataProvider_ValidationException;
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ScatoDbContext>());
            DefaultModelBinder.ResourceClassKey = "Errores";
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Errores";
            AreaRegistration.RegisterAllAreas();

            ValueProviderFactories.Factories.Insert(0, new CultureAwareQueryStringValueProviderFactory());

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Log4NetConfig.Configure(Server);    
        }

        void CookieTempDataProvider_ValidationException(object sender, Exception e)
        {

        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            var logger = LogManager.GetLogger(GetType());
            logger.Error("Excepción no manejada: ", ex);
        }
    }
}
