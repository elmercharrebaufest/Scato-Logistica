using System;
using System.Activities.Tracking;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Actividades.Behaviour
{
    public class WorkflowTrackingParticipant : TrackingParticipant
    {
        private readonly IServicioComandos servicioComandos;
        private readonly ILogger log;

        public WorkflowTrackingParticipant(IServicioComandos servicioComandos, ILogger log)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            var instanceRecord = record as WorkflowInstanceRecord;
            if (instanceRecord != null && instanceRecord.State == "Completed")
            {
                try
                {
                    log.Debug("Workflow completado: {0}", instanceRecord.InstanceId);
                    servicioComandos.Ejecutar(new FinDeWorkflow { InstanceId = instanceRecord.InstanceId });
                }
                catch (Exception e)
                {
                    log.Error(e, "Error al ejecutar el comando de fin de workflow de {0}", instanceRecord.InstanceId);
                    throw;
                }
            }
        }
    }
}
