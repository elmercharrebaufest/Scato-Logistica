using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IServicioSapZE7550Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ServicioSapZE7550(Guid instanceId, bool envioCola, ControlRecorridoDto controlRecorrido);
    }
}
