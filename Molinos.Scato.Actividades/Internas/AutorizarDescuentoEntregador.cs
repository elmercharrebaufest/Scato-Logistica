using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AutorizarDescuentoEntregador : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public InArgument<CaladoDto> Calado { get; set; }

        public OutArgument<bool> DecisionEntregador { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {

            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            var calado = Calado.Get<CaladoDto>(context);

            DecisionEntregador.Set(context, controlRecorrido.Decision);

            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new ModificarDecisionEntregadorCalado { CaladoId = calado.Id, Decision = controlRecorrido.Decision });

                if (controlRecorrido.Decision)
                {
                    servicioComandos.Ejecutar(new InformarCaladoCircular { WorkflowInstanceId = context.WorkflowInstanceId, EsPostCalado = true });
                }

            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}

