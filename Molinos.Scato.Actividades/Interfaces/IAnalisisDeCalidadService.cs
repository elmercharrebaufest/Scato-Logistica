using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IAnalisisDeCalidadService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado AnalisisDeCalidad(System.Guid instanceId, string numeroDeOrden, Molinos.Scato.Dominio.Dto.AnalisisPorCaracteristicaDto[] caracteristicasAnalizadas, ControlRecorridoDto controlRecorrido);
    }
}
