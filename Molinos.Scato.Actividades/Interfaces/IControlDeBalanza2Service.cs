using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IControlDeBalanza2Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ControlDeBalanza2(Guid instanceId, ControlRecorridoDto controlRecorrido, int balanzaId);
    }
}
