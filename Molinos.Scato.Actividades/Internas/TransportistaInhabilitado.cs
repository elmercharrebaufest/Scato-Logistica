using System.Activities;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class TransportistaInhabilitado : CodeActivity
    {

        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<string> Observacion { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            var control = ControlRecorrido.Get<ControlRecorridoDto>(context);
            Observacion.Set(context, control.Mensaje + "\n" + control.Comentario);
        }
    }
}
