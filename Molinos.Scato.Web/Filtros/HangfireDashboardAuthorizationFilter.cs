using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;

namespace Molinos.Scato.Web.Filtros
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            //var owinContext = new OwinContext(context.GetOwinEnvironment
            //owinContext.Authentication.User.Identity.IsAuthenticated;
            return true;
        }
    }
}