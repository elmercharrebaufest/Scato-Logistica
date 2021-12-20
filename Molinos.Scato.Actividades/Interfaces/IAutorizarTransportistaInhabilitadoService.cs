using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IAutorizarTransportistaInhabilitadoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        bool AutorizarTransportistaInhabilitado(Guid instanceId, bool autorizado, bool terminaInhabilitacion, ControlRecorridoDto controlRecorridoDto);
    }
}
