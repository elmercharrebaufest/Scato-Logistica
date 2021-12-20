using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IServicioSapMov291Service
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ServicioSapMov291(Guid instanceId, bool envioCola, ControlRecorridoDto controlRecorrido);
    }
}
