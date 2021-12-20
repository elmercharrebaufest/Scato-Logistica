using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IIngresarEmbarqueService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado IngresarEmbarque(EmbarqueDto embarque, string nombreWorkflow, int workflowDefinicionId, ControlRecorridoDto controlRecorrido);
    }
}
