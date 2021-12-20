using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface ILineUpService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado LineUp(ControlRecorridoDto controlRecorrido, LineUpDto lineup, Guid instanceId);
    }
}
