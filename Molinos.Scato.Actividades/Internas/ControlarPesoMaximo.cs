using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlarPesoMaximo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<bool> ExcedePesoMaximo { get; set; }
        public OutArgument<string> Observacion { get; set; }
        public OutArgument<bool> Repesar { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            Observacion.Set(context, controlRecorrido.Mensaje + "\n" + controlRecorrido.Comentario);
            ExcedePesoMaximo.Set(context, true);
            Repesar.Set(context, controlRecorrido.Decision);
            var servicioComando = context.GetExtension<IServicioComandos>();
            if (controlRecorrido.Decision)
            {
                var resultado =
                    servicioComando.Ejecutar(new ModificarRecorridoBalanzaBruto
                        {
                            InstanceId = controlRecorrido.WorkflowInstanceId
                        });
            }

        }
    }
}
