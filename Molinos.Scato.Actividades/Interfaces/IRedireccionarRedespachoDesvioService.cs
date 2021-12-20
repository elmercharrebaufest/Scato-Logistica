using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IRedireccionarRedespachoDesvioService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado CargarCartaPorteRedespachoDesvio(Guid instanceId, CartaPorteDto orden, VehiculoDto vehiculo);
    }
}
