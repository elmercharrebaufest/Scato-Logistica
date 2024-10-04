using System;
using System.Web;
using System.IdentityModel.Services;
using System.Text;
using System.Web.Mvc;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.WebMobile.Seguridad
{
    public class SameSiteCookieHandler : CookieHandler
    {
        private ILogger GetLogger()
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            var logger = loggerFactory.GetLogger(this.GetType());
            return logger;
        }

        protected override void DeleteCore(string name, string path, string domain, HttpContext context)
        {
            var logger = GetLogger();
            logger.Debug("Delete Cookie " + name + "");
            context.Response.Cookies.Remove(name);
        }

        protected override byte[] ReadCore(string name, HttpContext context)
        {
            var logger = GetLogger();
            HttpCookie cookie = context.Request.Cookies[name];
            if (cookie != null)
            {
                logger.Debug("Read Cookie " + name + " :" + cookie.Value);
                return Encoding.UTF8.GetBytes(cookie.Value);
            }

            logger.Debug("Read Cookie " + name + " : NULL" );


            return null;
        }

        protected override void WriteCore(byte[] value, string name, string path, string domain, DateTime expirationTime, bool secure, bool httpOnly, HttpContext context)
        {
            var txtValue = Encoding.UTF8.GetString(value);
            var cookie = new HttpCookie(name, txtValue);
            cookie.Path = path;
            cookie.Domain = domain;
            cookie.Expires = expirationTime;
            cookie.Secure = secure;
            cookie.HttpOnly = httpOnly;
            cookie.SameSite = SameSiteMode.Strict;
            context.Response.Cookies.Add(cookie);

            var logger = GetLogger();
            logger.Debug("Write Cookie " + name + " :" + txtValue);
        }
    }
}