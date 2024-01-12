using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class EliminarAsignacionAutomatismoGranoEnRecorrido : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var comandos = context.GetExtension<IServicioComandos>();
            var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
            comandos.Ejecutar(new Dominio.Comandos.EliminarAsignacionAutomatismoGranoEnRecorrido { RecorridoId = recorridoId });
        }
    }
}