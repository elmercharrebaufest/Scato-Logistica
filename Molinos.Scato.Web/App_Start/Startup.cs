using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Molinos.Scato.Web.App_Start;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Jobs;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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
            bool activarHangFire;
            bool.TryParse(ConfigurationManager.AppSettings["ActivarHangFire"],out activarHangFire);
            if (activarHangFire) {
                new HangfireJobs().InicializarJobs(app);
            }
        }
    }
}