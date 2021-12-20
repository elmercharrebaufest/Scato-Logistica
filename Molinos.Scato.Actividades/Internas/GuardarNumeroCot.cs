using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GuardarNumeroCot : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> NumeroCot { get; set; }
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var numero = NumeroCot.Get<string>(context);
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new ModificarRecorridoNumeroCot { InstanceId = context.WorkflowInstanceId, NumeroCot = numero });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.IngresoNumeroCot_Error);
            }
            return resultado;
        }
    }
}