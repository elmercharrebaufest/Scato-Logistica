using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IServicioSap_Z4030Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ServicioSap_Z4030(Guid instanceId, bool envioCola, ControlRecorridoDto controlRecorrido);
    }
}
