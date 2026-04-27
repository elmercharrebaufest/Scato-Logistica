using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Enums;
using System.ServiceModel;
using System.Threading.Tasks;

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

        [OperationContract]
        Task<HealthCheckResult> EjecutarHealthCheckAsync(string jobName);
        
        [OperationContract]
        void CachearCpeAFIPSanLorenzo();

        [OperationContract]
        void CachearCpeAFIPPorCentros();

        [OperationContract]
        void ActualizarCacheCpeAFIPSanLorenzo();

        [OperationContract]
        void LimpiarCacheCpeAFIPDocumentosIngresados();

        [OperationContract]
        void LimpiarCacheCpeAFIPDocumentosNoIngresados();
    }
}