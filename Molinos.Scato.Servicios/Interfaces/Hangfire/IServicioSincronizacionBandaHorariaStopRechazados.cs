using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioSincronizacionBandaHorariaStopRechazados
    {
        [OperationContract]
        void SincronizarBandaHorariaStop();
                
    }
}