using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IServicioSapFill_Z1000Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ServicioSapFill_Z1000(Guid instanceId, bool envioCola, ControlRecorridoDto controlRecorrido);
    }
}
