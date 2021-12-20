using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GuardarControlBalanza : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InOutArgument<bool> ControlBalanza { get; set; } // Se seteo el argumento In/Out para deprecar el flujo de control balanza a otra actividad

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = context.WorkflowInstanceId;
            var controlBalanza = ControlBalanza.Get<bool>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var resultado = new Resultado();
            try
            {
                if (controlBalanza)
                {
                    resultado =
                    servicioComandos.Ejecutar(new ModificarRecorridoControlBalanza
                    {
                        InstanceId = instanceId,
                        ControlBalanza = controlBalanza
                    });
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            context.SetValue(ControlBalanza, false); // Se seteo el argumento In/Out para deprecar el flujo de control balanza a otra actividad
            return resultado;
        }
    }
}