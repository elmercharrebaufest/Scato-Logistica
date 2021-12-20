using System.Web.Mvc;
using Molinos.Scato.WebOperaciones.Atributos;

namespace Molinos.Scato.WebOperaciones.App_Start
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