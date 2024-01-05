using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Molinos.Scato.Web.App_Start;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Jobs;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(Startup))]

namespace Molinos.Scato.Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ScatoDb"].ConnectionString;
            GlobalHost.DependencyResolver.UseSqlServer(connectionString);
            app.MapSignalR();

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            var dashboarOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", dashboarOptions);
            app.UseHangfireServer();

            new HangfireJobs().InicializarJobs();
        }
    }
}