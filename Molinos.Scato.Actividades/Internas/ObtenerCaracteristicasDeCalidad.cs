using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerCaracteristicasDeCalidad : CodeActivity
    {
        public OutArgument<AnalisisPorCaracteristicaDto[]> CaracteristicasDeCalidad { get; set; }
        public OutArgument<bool> TieneDescuentos { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();
            var caracteristicas = servicio.ListarAnalisisYCaladoPorCaracteristica(context.WorkflowInstanceId);
            CaracteristicasDeCalidad.Set(context, caracteristicas);
            TieneDescuentos.Set(context, caracteristicas.Any(x => x.DescuentoEnKg > 0 || x.DescuentoEnPorcentaje > 0));
        }
    }
}
