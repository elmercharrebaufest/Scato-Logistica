using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarPuestoComando : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public OutArgument<bool> EstaAsignado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var workflowId = WorkflowId.Get<Guid>(context);

            EstaAsignado.Set(context,repositorio.WorkflowEstaAsignado(workflowId));
        }
    }
}
