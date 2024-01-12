using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class EliminarAsignacionNoGranoEnRecorrido : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var comandos = context.GetExtension<IServicioComandos>();
            var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
            comandos.Ejecutar(new Dominio.Comandos.EliminarAsignacionNoGranoEnRecorrido { RecorridoId = recorridoId });
        }
    }
}