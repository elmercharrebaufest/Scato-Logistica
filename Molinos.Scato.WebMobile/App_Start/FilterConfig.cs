using System.Web.Mvc;
using Molinos.Scato.WebMobile.Atributos;

namespace Molinos.Scato.WebMobile.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CertificateHandleErrorAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AvoidCacheFilterAttribute());
        }
    }
}