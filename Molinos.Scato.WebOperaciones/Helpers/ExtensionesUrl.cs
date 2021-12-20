using System;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.WebOperaciones.Helpers
{
    public static class ExtensionesUrl
    {
        private static readonly string AssemblyVersionParams = "?v=" + typeof (MvcApplication).Assembly.GetName().Version;
        
        public static string RelativeToAbsolute(string severPath)
        {
            return AsClientUrl(VirtualPathUtility.ToAbsolute(severPath));
        }

        private static string AsClientUrl(string absolutePath)
        {
            var builder = new UriBuilder(HttpContext.Current.Request.Url) { Path = absolutePath, Query = "" };
            builder.Scheme = "http";
            #if (DEBUG)
                builder.Port = 56683;
            #endif
            return builder.Uri.AbsoluteUri;
        }

        public static string Script(this UrlHelper helper,string url)
        {
            return helper.Content(url + AssemblyVersionParams);
        }

        public static string Css(this UrlHelper helper, string url)
        {
            return helper.Content(url + AssemblyVersionParams);
        }
    }
}
