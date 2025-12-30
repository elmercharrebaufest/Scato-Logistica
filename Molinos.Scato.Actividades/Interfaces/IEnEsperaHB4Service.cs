using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IEnEsperaHB4Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado EnEsperaHB4(ControlRecorridoDto controlRecorrido, Guid instanceId, bool rechazar);
    }
}
