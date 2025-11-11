using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioHangfireQueue
    {
        [OperationContract]
        void EncolarImportarCartaPorteVisec(int id);
    }
}