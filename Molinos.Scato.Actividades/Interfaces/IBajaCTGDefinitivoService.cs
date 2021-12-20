using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IBajaCTGDefinitivoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado BajaCTGDefinitivo(Guid instanceId, DecisionCtg decision, ControlRecorridoDto controlRecorrido);
    }
}
