using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCamionRechazado : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<string> Observacion { get; set; }
        public OutArgument<bool> Rechazar { get; set; }
        public OutArgument<bool> RevierteRechazoVagon { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            Observacion.Set(context, controlRecorrido.Mensaje + "\n" + controlRecorrido.Comentario);
            Rechazar.Set(context, controlRecorrido.Decision);
            RevierteRechazoVagon.Set(context, controlRecorrido.RevierteRechazo);
            if (controlRecorrido.RevierteRechazo)
            {
                servicio.Ejecutar(new ModificarRecorridoPeso
                {
                    InstanceId = context.WorkflowInstanceId,
                    TipoPesada = TipoPesada.Bruto,
                    Peso = null,
                    BalanzaId = 0
                });

                servicio.Ejecutar(new ModificarRecorridoPeso
                {
                    InstanceId = context.WorkflowInstanceId,
                    TipoPesada = TipoPesada.Tara,
                    Peso = null,
                    BalanzaId = 0,
                    Usuario = controlRecorrido.NombreUsuario
                });
            }

        }
    }
}
