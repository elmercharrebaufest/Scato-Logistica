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

        /// <summary>
        /// Versión liviana de <see cref="CachearCpeAFIPSanLorenzo"/>: solo consulta AFIP y persiste
        /// en CartaPorteElectronica (comando CachearCPEAfip), sin resolver Proveedores/Localidad/
        /// Categoria/Chofer/Transportista ni renderizar el PDF a PNG. Mismo filtro de CTGs no
        /// cacheadas, paralelismo y reintentos que la versión original.
        /// </summary>
        [OperationContract]
        void CachearCpeAFIPSanLorenzoLiviano();

        /// <summary>
        /// Versión liviana de <see cref="CachearCpeAFIPPorCentros"/>: solo consulta AFIP y persiste
        /// en CartaPorteElectronica (comando CachearCPEAfip), sin resolver Proveedores/Localidad/
        /// Categoria/Chofer/Transportista ni renderizar el PDF a PNG. Mismo filtro de CTGs no
        /// cacheadas, paralelismo y reintentos que la versión original.
        /// </summary>
        [OperationContract]
        void CachearCpeAFIPPorCentrosLiviano();

        [OperationContract]
        void ActualizarCacheCpeAFIPSanLorenzo();

        [OperationContract]
        void LimpiarCacheCpeAFIPDocumentosIngresados();

        [OperationContract]
        void LimpiarCacheCpeAFIPDocumentosNoIngresados();
    }
}