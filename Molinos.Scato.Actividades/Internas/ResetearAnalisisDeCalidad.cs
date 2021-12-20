using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ResetearAnalisisDeCalidad : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var instanceId = context.WorkflowInstanceId;
            var servicioComandos = context.GetExtension<IServicioComandos>();

            servicioComandos.Ejecutar(new ModificarRecorridoAnalisisDeCalidad { InstanceId = instanceId });
        }
    }
}
