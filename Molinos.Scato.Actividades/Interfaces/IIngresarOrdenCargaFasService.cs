using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IIngresarOrdenCargaFasService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado IngresarOrdenCargaFas(OrdenCargaFasDto orden, int centroId, string nombreWorkflow, int workflowDefinicionId, bool validaCompliance, string usuario, ControlRecorridoDto controlRecorrido);
    }
}
