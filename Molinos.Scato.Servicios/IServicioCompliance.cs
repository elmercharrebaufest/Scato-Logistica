using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioCompliance
    {
        [OperationContract]
        void ControlarDatos(string cuit, string nroDocumento, string patente1, string patente2);
    }
}
