using System.Activities;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlarPeso : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<string> Observacion { get; set; }
        public OutArgument<bool> Rechazado { get; set; }
        public OutArgument<bool> ExistioDiferenciaPesoNeto { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            Observacion.Set(context, controlRecorrido.Mensaje + "\n" + controlRecorrido.Comentario);
            Rechazado.Set(context, !controlRecorrido.Decision);
            ExistioDiferenciaPesoNeto.Set(context, true);
        }
    }
}
