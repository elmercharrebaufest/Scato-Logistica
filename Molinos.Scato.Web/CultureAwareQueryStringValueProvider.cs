using System.Globalization;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Molinos.Scato.Web
{
    public sealed class CultureAwareQueryStringValueProvider : NameValueCollectionValueProvider
    {
        public CultureAwareQueryStringValueProvider(ControllerContext controllerContext)
            : base(controllerContext.HttpContext.Request.QueryString, controllerContext.HttpContext.Request.Unvalidated().QueryString, CultureInfo.CurrentUICulture)
        {
        }
    }
}