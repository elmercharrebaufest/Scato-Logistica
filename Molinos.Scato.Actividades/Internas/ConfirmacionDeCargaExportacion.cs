using System.Activities;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class ConfirmacionDeCargaExportacion : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<bool> CargaParcial { get; set; }
        public OutArgument<bool> Cancelado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            CargaParcial.Set(context, controlRecorrido.Decision);
            Cancelado.Set(context, controlRecorrido.Mensaje == "Cancelado");

        }
    }
}
