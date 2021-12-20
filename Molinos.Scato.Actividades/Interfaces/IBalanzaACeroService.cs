using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IBalanzaACeroService
    {
        [OperationContract]
        void BalanzaACero(Guid instanceId, ControlRecorridoDto controlRecorrido);
    }
}
