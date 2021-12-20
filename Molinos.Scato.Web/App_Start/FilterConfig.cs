using System.Web.Mvc;
using Molinos.Scato.Web.Atributos;

namespace Molinos.Scato.Web.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AvoidCacheFilterAttribute());
        }
    }
}