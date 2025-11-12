using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioSincronizacionPay
    {
        [OperationContract]
        void SincronizarMOAPayEstadoDePagos();

        [OperationContract]
        void SincronizarMOAPayCPE();

        [OperationContract]
        void SincronizarMOAPayOperacionesFason();

        [OperationContract]
        void SincronizarMOAPayOperacionesFas();

        [OperationContract]
        void SincronizarMOAPayOperacionesResiduos();
    }
}