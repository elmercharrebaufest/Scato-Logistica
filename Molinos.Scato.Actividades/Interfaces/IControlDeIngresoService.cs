using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IControlDeIngresoService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ControlDeIngreso(OrdenDeDescargaFasonDto orden, bool rechazar, ControlRecorridoDto controlRecorrido, Guid instanceId);
    }
}
