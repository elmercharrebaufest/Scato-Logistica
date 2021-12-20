using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerTarjetaAsignadaEngarita : CodeActivity
    {
        public OutArgument<string> Tarjeta { get; set; }

        public OutArgument<bool> TieneTarjeta { get; set; }

        public OutArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();
            var instanceId = context.WorkflowInstanceId;
            var tarjeta= servicio.ObtenerTarjetaPorCargaDeCupo(instanceId);
            ControlRecorrido.Set(context, new ControlRecorridoDto
            {
                Actividad = Textos.ActAsignacionTarjetaDeAcceso,
                ActividadXaml = "AsignacionTarjetaDeAcceso",
                WorkflowInstanceId = context.WorkflowInstanceId,
                PuestoDeTrabajoId = 0,
                NombreUsuario = string.Empty
            });
            Tarjeta.Set(context, tarjeta ?? string.Empty);
            TieneTarjeta.Set(context, tarjeta != null);
        }
    }
}
