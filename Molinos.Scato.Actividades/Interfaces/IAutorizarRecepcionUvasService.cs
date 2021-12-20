using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IAutorizarRecepcionUvasService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        bool AutorizarRecepcionUvas(Guid instanceId, ControlRecorridoDto controlRecorridoDto);
    }
}
