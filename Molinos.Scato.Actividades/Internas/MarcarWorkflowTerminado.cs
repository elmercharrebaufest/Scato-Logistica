using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class MarcarWorkflowTerminado : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();

                var instanceId = InstanceId.Get<Guid>(context);

                resultado = servicioComandos.Ejecutar(new InformarSalidaCircular { WorkflowInstanceId = instanceId });
                resultado = servicioComandos.Ejecutar(new ModificarRecorridoTerminado { InstanceId = instanceId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Recorrido_ErrorAlTerminar);
            }
            return resultado;
        }
    }
}
