using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Configuration;

namespace Molinos.Scato.ServiciosWeb.Configurations
{
    public static class HangfireConfiguration
    {
        public static void StartServer(IAppBuilder app)
        {
            var options = new SqlServerStorageOptions
            {
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero
            };
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["ScatoHangfireDb"].ConnectionString, options);

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            GlobalConfiguration.Configuration.UseSerializerSettings(jsonSettings);

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            app.UseHangfireServer();

            var dashboarOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", dashboarOptions);
        }

        public static void StartJobs()
        {
            StartJobsForAutomatismos();
            StartJobsForMOAPay();
        }

        private static void StartJobsForAutomatismos()
        {
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoGrano, x => x.Llamar(LlamadoAutomatico.Granos), "*/30 * * * * *");
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoNoGrano, x => x.Llamar(LlamadoAutomatico.NoGranos), Cron.MinuteInterval(1));
        }

        private static void StartJobsForMOAPay()
        {
            var cronExpressionForSincronizarMOAPayEstadoDePagos = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.SincronizarMOAPayEstadoDePagos"];
            if (string.IsNullOrWhiteSpace(cronExpressionForSincronizarMOAPayEstadoDePagos))
                cronExpressionForSincronizarMOAPayEstadoDePagos = Constantes.Job.DefaultCronExpressionForSincronizarMOAPayEstadoDePagos;

            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.SincronizarMOAPayEstadoDePagos, x => x.SincronizarMOAPayEstadoDePagos(), cronExpressionForSincronizarMOAPayEstadoDePagos);

            bool.TryParse(ConfigurationManager.AppSettings["Hangfire.IsEnabled.SincronizarMOAPayCPE"], out bool isEnabledSincronizarMOAPayCPE);
            if (isEnabledSincronizarMOAPayCPE)
                RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.SincronizarMOAPayCPE, x => x.SincronizarMOAPayCPE(), "0 30 * * * *");
        }
    }

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