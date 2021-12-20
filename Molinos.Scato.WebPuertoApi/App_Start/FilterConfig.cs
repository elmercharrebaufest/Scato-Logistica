using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.WebPuertoApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
