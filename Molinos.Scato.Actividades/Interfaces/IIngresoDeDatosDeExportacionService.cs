using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IIngresoDeDatosDeExportacionService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado IngresoDeDatosDeExportacion(Guid instanceId, ControlRecorridoDto controlRecorrido, IngresoDeDatosDeExportacionDto ingresoDeDatosDeExportacion);
    }

}
