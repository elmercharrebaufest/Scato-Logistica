using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IIngresoDeLoteService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado IngresoDeLote(Guid instanceId, string loteNro, int almacenId, ControlRecorridoDto controlRecorrido);
    }
}
