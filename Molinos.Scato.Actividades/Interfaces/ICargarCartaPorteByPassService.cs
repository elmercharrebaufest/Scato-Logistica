using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface ICargarCartaPorteByPassService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado CargarCartaPorteByPass(CartaPorteDto orden, VehiculoDto vehiculo, int centroId, string nombreWorkflow, int workflowDefinicionId, string usuario, ControlRecorridoDto controlRecorrido);
    }
}
