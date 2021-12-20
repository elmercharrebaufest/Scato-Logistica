using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Molinos.Scato.WebPuertoApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            EnableCrossSiteRequests(config);
            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute("*", "*", "GET, POST, PUT, DELETE, OPTIONS") { SupportsCredentials = true });
        }
    }

}
