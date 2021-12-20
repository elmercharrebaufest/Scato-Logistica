using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IEmpezarNuevoWorkflowService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado EmpezarNuevoWorkflow(Guid instanceIdOrigen, int centroId, string nombreWorkflow, int workflowDefinicionId, string usuario);
    }
}
