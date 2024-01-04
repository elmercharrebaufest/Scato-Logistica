using Hangfire;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Web.Jobs
{
    public class HangfireJobs
    {
        public void InicializarJobs()
        {
            JobLlamadoAutomaticoGranos();
            JobLlamadoAutomaticoNoGranos();
        }

        public void JobLlamadoAutomaticoGranos()
        {
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.DetenerAutomatismoGrano, x => x.Detener(LlamadoAutomatico.Granos), Cron.MinuteInterval(4));
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoGrano, x => x.Llamar(LlamadoAutomatico.Granos), Cron.MinuteInterval(4));
        }

        public void JobLlamadoAutomaticoNoGranos()
        {
            RecurringJob.AddOrUpdate<IServicioLlamadoAutomatico>(Constantes.Job.LlamarAutomatismoNoGrano, x => x.Llamar(LlamadoAutomatico.NoGranos), Cron.MinuteInterval(4));
        }
    }
}