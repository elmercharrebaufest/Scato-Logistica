using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GuardarDatosProximaActividad : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Dato { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var dato = Dato.Get<string>(context);
                var servicioComandos = context.GetExtension<IServicioComandos>();

                servicioComandos.Ejecutar(new ModificarRecorridoDatosProximaActividad { InstanceId = context.WorkflowInstanceId, DatosProximaActividad = dato });
                context.GetExtension<ScatoPersistenceParticipant>().DatosProximaActividad = dato;
            }
            catch
            {
            }
        }
    }
}
