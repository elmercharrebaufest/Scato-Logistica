using System.Collections.Generic;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Filtros
{
    public class ViewBagToResponseHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            IDictionary<string, string> headers = filterContext.Controller.ViewBag.Headers;
            if (headers != null)
            {
                var response = filterContext.HttpContext.Response;
                foreach (var header in headers)
                {
                    response.AddHeader(header.Key, header.Value);
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}