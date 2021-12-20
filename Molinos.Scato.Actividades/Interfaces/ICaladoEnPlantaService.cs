using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface ICaladoEnPlantaService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado CaladoEnPlanta(System.Guid instanceId, CaladoEnPlantaDto calado, ControlRecorridoDto controlRecorrido);
    }
}
