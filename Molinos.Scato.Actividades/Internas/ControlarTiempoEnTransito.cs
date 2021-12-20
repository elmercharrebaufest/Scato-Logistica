using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlarTiempoEnTransito : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<bool> Autoriza { get; set; }
        public OutArgument<string> Observacion { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servComando = context.GetExtension<IServicioComandos>();
            var control = ControlRecorrido.Get<ControlRecorridoDto>(context);
            servComando.Ejecutar(new CrearControlRecorrido()
            {
                Dto = control
            });
            Observacion.Set(context, control.Mensaje + "\n" + control.Comentario);
            Autoriza.Set(context, control.Decision);
        }
    }
}
