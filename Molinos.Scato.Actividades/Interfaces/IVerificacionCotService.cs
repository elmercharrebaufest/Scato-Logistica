using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IVerificacionCotService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado VerificacionCot(Guid instanceId, ControlRecorridoDto controlRecorrido, bool esCotManual, string numeroCot);
    }
}
