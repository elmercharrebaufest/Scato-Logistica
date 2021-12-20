using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IControlPesoMaximoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ControlPesoMaximo(ControlRecorridoDto controlRecorrido, Guid instanceId);
    }
}
