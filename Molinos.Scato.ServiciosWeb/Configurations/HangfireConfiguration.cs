using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPOI.SS.Formula.Functions;
using Owin;
using System;
using System.Configuration;
using Ninject;
using Ninject.Extensions.Logging;

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
            StartJobsForVisec();
            StartJobsHealthChecks();
            StartJobsCacheCpeAfip();
            StartJobsLimpiezaCacheCpe();            
            StartJobsForStop();
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

            RecurringJob.AddOrUpdate<IServicioSincronizacionPay>(Constantes.Job.SincronizarMOAPayEstadoDePagos, x => x.SincronizarMOAPayEstadoDePagos(), cronExpressionForSincronizarMOAPayEstadoDePagos);

            if (ValidarEncendidoJob("Hangfire.IsEnabled.SincronizarMOAPayCPE"))
            {
                var cronExpressionForSincronizarMOAPayCPE = ObtenerCronJob("Hangfire.CronExpressionFor.SincronizarMOAPayCPE");
                RecurringJob.AddOrUpdate<IServicioSincronizacionPay>(Constantes.Job.SincronizarMOAPayCPE, x => x.SincronizarMOAPayCPE(), cronExpressionForSincronizarMOAPayCPE);
            }

            if (ValidarEncendidoJob("Hangfire.IsEnabled.SincronizarMOAPayOperacionesFason"))
            {
                var cronExpressionForSincronizarMOAPayOperacionesFason = ObtenerCronJob("Hangfire.CronExpressionFor.SincronizarMOAPayOperacionesFason");
                RecurringJob.AddOrUpdate<IServicioSincronizacionPay>(Constantes.Job.SincronizarMOAPayOperacionesFason, x => x.SincronizarMOAPayOperacionesFason(), cronExpressionForSincronizarMOAPayOperacionesFason);
            }

            if (ValidarEncendidoJob("Hangfire.IsEnabled.SincronizarMOAPayOperacionesFas"))
            {
                var cronExpressionForSincronizarMOAPayOperacionesFas = ObtenerCronJob("Hangfire.CronExpressionFor.SincronizarMOAPayOperacionesFas");
                RecurringJob.AddOrUpdate<IServicioSincronizacionPay>(Constantes.Job.SincronizarMOAPayOperacionesFas, x => x.SincronizarMOAPayOperacionesFas(), cronExpressionForSincronizarMOAPayOperacionesFas);
            }

            if (ValidarEncendidoJob("Hangfire.IsEnabled.SincronizarMOAPayOperacionesResiduos"))
            {
                var cronExpressionForSincronizarMOAPayOperacionesResiduos = ObtenerCronJob("Hangfire.CronExpressionFor.SincronizarMOAPayOperacionesResiduos");
                RecurringJob.AddOrUpdate<IServicioSincronizacionPay>(Constantes.Job.SincronizarMOAPayOperacionesResiduos, x => x.SincronizarMOAPayOperacionesResiduos(), cronExpressionForSincronizarMOAPayOperacionesResiduos);
            }
        }

        private static void StartJobsForStop()
        {   
            if (ValidarEncendidoJob("Hangfire.IsEnabled.SincronizarBandaHorariaStopRechazados"))
            {
                var cronExpressionForHangfireSincronizarBandaHorariaStopRechazados = ObtenerCronJob("Hangfire.CronExpressionFor.SincronizarBandaHorariaStopRechazados");
                RecurringJob.AddOrUpdate<IServicioSincronizacionBandaHorariaStopRechazados>(Constantes.Job.SincronizarBandaHorariaStopRechazados, x => x.SincronizarBandaHorariaStop(), cronExpressionForHangfireSincronizarBandaHorariaStopRechazados);
            }

        }

        private static bool ValidarEncendidoJob(string configEnabled)
        {
            return bool.TryParse(ConfigurationManager.AppSettings[configEnabled], out bool isEnabled) && isEnabled;
        }

        private static string ObtenerCronJob(string configCron)
        {
            var cronExpression = ConfigurationManager.AppSettings[configCron];
            return string.IsNullOrWhiteSpace(cronExpression) ? Constantes.Job.DefaultCronExpressionForSincronizarMOAPayEstadoDePagos : cronExpression;
        }

        private static void StartJobsForVisec()
        {
            var cronExpressionForSincronizarEstadoTransmisionVisec = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.SincronizarEstadoTransmisionVisec"];
            if (string.IsNullOrWhiteSpace(cronExpressionForSincronizarEstadoTransmisionVisec))
                cronExpressionForSincronizarEstadoTransmisionVisec = "0 * * * *";

            bool.TryParse(ConfigurationManager.AppSettings["Hangfire.IsEnabled.SincronizarEstadoTransmisionVisec"], out bool isEnabledSincronizarEstadoTransmisionVisec);
            if (isEnabledSincronizarEstadoTransmisionVisec)
                RecurringJob.AddOrUpdate<IServicioSincronizacionVisec>(Constantes.Job.SincronizarEstadoTransmisionVisec, x => x.SincronizarEstadoTransmision(), cronExpressionForSincronizarEstadoTransmisionVisec);
        }
        
        private static void StartJobsHealthChecks()
        {
            var cronForHealthChecks = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.HealthCheck.MOAPay"];
            if (string.IsNullOrWhiteSpace(cronForHealthChecks))
                cronForHealthChecks = Constantes.Job.DefaultCronExpressionForSincronizarMOAPayEstadoDePagos;

            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.VerificarHealthCheckMOAPayHealth, x => x.EjecutarHealthCheckAsync(Constantes.Job.VerificarHealthCheckMOAPayHealth), cronForHealthChecks);
        }

        private static void StartJobsCacheCpeAfip()
        {
            var cronExpressionForCachearCpeAFIPSanLorenzos = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.CachearCpeAFIPSanLorenzo"];
            if (string.IsNullOrWhiteSpace(cronExpressionForCachearCpeAFIPSanLorenzos))
                cronExpressionForCachearCpeAFIPSanLorenzos = Constantes.Job.DefaultCronExpressionForCachearCpeAFIPSanLorenzo;

            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.CachearCpeAFIPSanLorenzo, x => x.CachearCpeAFIPSanLorenzo(), cronExpressionForCachearCpeAFIPSanLorenzos);

            var cronExpressionForCachearCpeAFIPPorCentros = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.CachearCpeAFIPPorCentros"];
            if (string.IsNullOrWhiteSpace(cronExpressionForCachearCpeAFIPPorCentros))
                cronExpressionForCachearCpeAFIPPorCentros = Constantes.Job.DefaultCronExpressionForCachearCpeAFIPPorCentros;

            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.CachearCpeAFIPPorCentros, x => x.CachearCpeAFIPPorCentros(), cronExpressionForCachearCpeAFIPPorCentros);

            var cronExpressionForActualizarCacheCpeAFIPSanLorenzo = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.ActualizarCacheCpeAFIPSanLorenzo"];
            if (string.IsNullOrWhiteSpace(cronExpressionForActualizarCacheCpeAFIPSanLorenzo))
                cronExpressionForActualizarCacheCpeAFIPSanLorenzo = Constantes.Job.DefaultCronExpressionForActualizarCacheCpeAFIPSanLorenzo;

            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.ActualizarCacheCpeAFIPSanLorenzo, x => x.ActualizarCacheCpeAFIPSanLorenzo(), cronExpressionForActualizarCacheCpeAFIPSanLorenzo);
        }

        private static void StartJobsLimpiezaCacheCpe()
        {
            if (ValidarEncendidoJob("Hangfire.IsEnabled.LimpiarCacheCartaPorteElectronicaDocumentosIngresados"))
            {
                var cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.LimpiarCacheCartaPorteElectronicaDocumentosIngresados"];
                if (string.IsNullOrWhiteSpace(cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados))
                    cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados = Constantes.Job.DefaultCronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados;

                RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LimpiarCacheCartaPorteElectronicaDocumentosIngresados, x => x.LimpiarCacheCpeAFIPDocumentosIngresados(), cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados);
            }

            if (ValidarEncendidoJob("Hangfire.IsEnabled.LimpiarCacheCartaPorteElectronicaDocumentosNoIngresados"))
            {
                var cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados = ConfigurationManager.AppSettings["Hangfire.CronExpressionFor.LimpiarCacheCartaPorteElectronicaDocumentosNoIngresados"];
                if (string.IsNullOrWhiteSpace(cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados))
                    cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados = Constantes.Job.DefaultCronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados;

                RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LimpiarCacheCartaPorteElectronicaDocumentosNoIngresados, x => x.LimpiarCacheCpeAFIPDocumentosNoIngresados(), cronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados);
            }
        }
    }

    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}