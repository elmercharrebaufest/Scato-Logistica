using Molinos.Scato.Dominio.Comandos;
using System;
using System.ServiceModel;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface IImportacionEgresoVisecService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado ImportacionEgresoVisec(Guid instanceId);
    }
}