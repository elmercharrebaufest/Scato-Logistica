using System.Activities;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class AutorizacionRecepcionUvas : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<string> Observacion { get; set; }
        public OutArgument<bool> Reintentar { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            Observacion.Set(context, controlRecorrido.Mensaje);
            Reintentar.Set(context, controlRecorrido.Decision);
        }
    }
}
