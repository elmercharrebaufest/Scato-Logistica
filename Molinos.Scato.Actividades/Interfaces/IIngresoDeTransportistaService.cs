using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IIngresoDeTransportistaService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado IngresoDeTransportista(TransportistaDto transportista, Guid instanceId, ControlRecorridoDto controlRecorrido);
    }
}
