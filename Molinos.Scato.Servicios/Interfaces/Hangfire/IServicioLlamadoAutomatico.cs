using System.ServiceModel;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioLlamadoAutomatico
    {
        [OperationContract]
        void Llamar(LlamadoAutomatico tipoLlamadoAutomatico);

        [OperationContract]
        void SincronizarMOAPayEstadoDePagos();

        [OperationContract]
        void SincronizarMOAPayCPE();
    }
}