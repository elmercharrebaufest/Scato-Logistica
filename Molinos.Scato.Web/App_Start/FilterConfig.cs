using Molinos.Scato.Web.Atributos;
using System.Web.Mvc;

namespace Molinos.Scato.Web.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AiHandleErrorAttribute());
            filters.Add(new AvoidCacheFilterAttribute());
        }
    }
}
