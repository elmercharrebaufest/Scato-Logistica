using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IAsignacionDeEstablecimientoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado AsignacionDeEstablecimiento(Guid instanceId, int? establecimiento, ControlRecorridoDto controlRecorrido, bool establecimientoPendiente);
    }
}
