using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.AfipCTGWebService;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IAccesoWsCtg
    {
        [OperationContract]
        authType ObtenerAuthType(string cuitRepresentado, Resultado resultado);
        [OperationContract]
        Auth ObtenerAuth(string cuitRepresentado, Resultado resultado);
    }
}
