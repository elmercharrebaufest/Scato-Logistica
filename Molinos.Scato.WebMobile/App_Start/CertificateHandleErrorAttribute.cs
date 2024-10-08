using System;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.App_Start
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class CertificateHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            if (filterContext.ExceptionHandled || 
                !typeof(InvalidOperationException).IsInstanceOfType(exception) || 
                exception.Source != "System.IdentityModel.Services" || 
                !exception.Message.StartsWith("ID1059"))
            {
                return;                
            }

            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("~/ErrorPages/ErrorEnCertificado.html");
        }
    }
}