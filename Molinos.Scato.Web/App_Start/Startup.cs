using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Molinos.Scato.Web.App_Start;
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

            bool.TryParse(ConfigurationManager.AppSettings["ActivarHangFire"], out bool activarHangFire);
            if (activarHangFire)
            {
                new HangfireJobs().InicializarJobs(app);
            }
        }
    }
}