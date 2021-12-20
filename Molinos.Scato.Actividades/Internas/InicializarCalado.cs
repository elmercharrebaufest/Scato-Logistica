using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public sealed class InicializarCalado : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            servicioComandos.Ejecutar(new ModificarRecorridoCalado { WorkflowInstanceId = context.WorkflowInstanceId});
        }
    }
}
