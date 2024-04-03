using Hangfire;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Filtros;
using Owin;

namespace Molinos.Scato.Web.Jobs
{
    public class HangfireJobs
    {
        public void InicializarJobs(IAppBuilder app)
        {
            Configurar(app);

            JobLlamadoAutomaticoGranos();
            JobLlamadoAutomaticoNoGranos();
        }

        private void Configurar(IAppBuilder app)
        {
            var dashboarOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", dashboarOptions);
            app.UseHangfireServer();
        }

        private void JobLlamadoAutomaticoGranos()
        {
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoGrano, x => x.Llamar(LlamadoAutomatico.Granos), "*/30 * * * * *");
        }

        private void JobLlamadoAutomaticoNoGranos()
        {
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoNoGrano, x => x.Llamar(LlamadoAutomatico.NoGranos), Cron.MinuteInterval(1));
        }
    }
}