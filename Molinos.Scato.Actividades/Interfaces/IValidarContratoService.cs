using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IValidarContratoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ValidarContrato(Guid instanceId, string contrato, bool omitir, ControlRecorridoDto controlRecorrido);
    }
}
