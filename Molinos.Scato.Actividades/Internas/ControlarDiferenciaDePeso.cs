using System.Activities;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlarDiferenciaDePeso : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<bool> Repesar { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            Repesar.Set(context, controlRecorrido.Decision);
        }
    }
}
