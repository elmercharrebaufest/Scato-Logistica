using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AceptarCamion : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }

        public OutArgument<bool> Rechazado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var workflowInstanceId = WorkflowInstanceId.Get<Guid>(context);
            
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.AceptarCamion {WorkflowId = workflowInstanceId});
                Rechazado.Set(context,false);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Recrorrido_ErrorEnLaCarga);
                Rechazado.Set(context,false);
            }

            return resultado;
        }
    }
}
