using Microsoft.Owin;
using Molinos.Scato.ServiciosWeb.Configurations;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(Molinos.Scato.ServiciosWeb.App_Start.Startup))]

namespace Molinos.Scato.ServiciosWeb.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            bool.TryParse(ConfigurationManager.AppSettings["ActivarHangFire"], out bool activarHangFire);
            if (activarHangFire)
            {
                HangfireConfiguration.StartServer(app);
                HangfireConfiguration.StartJobs();
            }
        }
    }
}