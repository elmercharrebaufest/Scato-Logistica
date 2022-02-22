using System.Activities;
using Molinos.Scato.Dominio.Dto;

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
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            Observacion.Set(context, controlRecorrido.Mensaje + "\n" + controlRecorrido.Comentario);
            Rechazar.Set(context, controlRecorrido.Decision);
            RevierteRechazoVagon.Set(context, controlRecorrido.RevierteRechazo);
        }
    }
}
