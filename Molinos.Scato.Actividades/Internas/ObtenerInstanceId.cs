using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    /// Expone el Id de Instancia del workflow. Este Id es usado como correlation handle en los workflows, por ser de fácil acceso.
    /// </summary>
    public class ObtenerInstanceId : CodeActivity<Guid>
    {
        protected override Guid Execute(CodeActivityContext context)
        {
            return context.WorkflowInstanceId;
        }
    }
}