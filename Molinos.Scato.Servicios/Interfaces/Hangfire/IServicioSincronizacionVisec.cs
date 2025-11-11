using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioSincronizacionVisec
    {
        [OperationContract]
        string SincronizarEstadoTransmision();
    }
}