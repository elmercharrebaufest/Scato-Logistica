using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class LeerToleranciaRechazo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<int> ToleranciaRechazo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var workflowInstanceId = InstanceId.Get<Guid>(context);
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var tolerancia = servicioRepositorio.LeerToleranciaRechazo(workflowInstanceId);

                ToleranciaRechazo.Set(context, tolerancia);

                var servicioComandos = context.GetExtension<IServicioComandos>();
                var resultado = servicioComandos.Ejecutar(new Dominio.Comandos.AceptarCamion { WorkflowId = workflowInstanceId });
            }
            catch
            {
                
            }
        }
    }
}
