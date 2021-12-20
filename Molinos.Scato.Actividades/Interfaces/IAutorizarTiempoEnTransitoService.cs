using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IAutorizarTiempoEnTransitoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado AutorizarTiempoEnTransito(AutorizacionTiempoEnTransitoDto autorizacionTiempoEnTransito, Guid instanceId, ControlRecorridoDto controlRecorrido, bool autorizado);
    }
}
