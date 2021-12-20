using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IVerificacionSalidaFleteService
    {
        [OperationContract]
        void VerificacionSalidaFlete(Guid instanceId, bool reintentar, ControlRecorridoDto controlRecorrido);
    }
}
