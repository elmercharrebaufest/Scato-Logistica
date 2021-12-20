using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IPesadaService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado Pesada(Guid instanceId, int peso, int almacenId, int? hidraulicaId, int? calleId, int balanzaId, int? proximaBalanzaId, bool controlBalanza, DateTime fecha, ControlRecorridoDto controlRecorrido);
    }
}
