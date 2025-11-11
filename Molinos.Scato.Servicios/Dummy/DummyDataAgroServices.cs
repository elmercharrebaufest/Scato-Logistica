using Molinos.Scato.Dominio;
using Molinos.Scato.Servicios.DataAgroService;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Molinos.Scato.Servicios.Dummy
{
    public class DummyDataAgroServices : IDataAgroServices
    {
        private readonly IServicioRepositorio _servicioRepositorio;

        public DummyDataAgroServices(IServicioRepositorio servicioRepositorio)
        {
            _servicioRepositorio = servicioRepositorio;
        }

        public ResultadoSap ActualizarCampaniaMaterial(CampaniaMaterialSAPDTO[] oCampaniaMaterialSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarCampaniaMaterialAsync(CampaniaMaterialSAPDTO[] oCampaniaMaterialSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ActualizarCesionContratoSAP(string contratoSAP, bool cesion)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarCesionContratoSAPAsync(string contratoSAP, bool cesion)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ActualizarContratoSAP(ContratoSAPDto contratoSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarContratoSAPAsync(ContratoSAPDto contratoSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ActualizarCupoSAP(CupoSapDto cupoSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarCupoSAPAsync(CupoSapDto cupoSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ActualizarEstadoComercial(InformeComercialSAPDTO[] oInformeComercialSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarEstadoComercialAsync(InformeComercialSAPDTO[] oInformeComercialSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ActualizarFijacionSAP(FijacionSAPDto fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ActualizarFijacionSAPAsync(FijacionSAPDto fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoAltaCampoSustentable AltaCampoSustentable(CampoDetalleTerceroDto campo)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoAltaCampoSustentable> AltaCampoSustentableAsync(CampoDetalleTerceroDto campo)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AltaContratoSAP(ContratoSAPDto contratoSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AltaContratoSAPAsync(ContratoSAPDto contratoSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AltaCupoSAP(CupoSapDto cupoSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AltaCupoSAPAsync(CupoSapDto cupoSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AltaFijacionSAP(FijacionSAPDto fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AltaFijacionSAPAsync(FijacionSAPDto fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AnulaFijacionVirtualSAP(FijacionVirtualSAP fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AnulaFijacionVirtualSAPAsync(FijacionVirtualSAP fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AnularContratoSAP(ContratoSAP contratoSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AnularContratoSAPAsync(ContratoSAP contratoSAP)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap AnularFijacionSAP(FijacionSAP fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> AnularFijacionSAPAsync(FijacionSAP fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public SISA[] BuscarProveedorEnSisa(string cuit)
        {
            throw new NotImplementedException();
        }

        public Task<SISA[]> BuscarProveedorEnSisaAsync(string cuit)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap ConfirmarFijacionSAP(string fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> ConfirmarFijacionSAPAsync(string fijacionSAP)
        {
            throw new NotImplementedException();
        }

        public CupoSapTerceroDto DatosCupoSap(string cupoSap)
        {
            var configuracionGeneral = _servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ConsultaDataAgroVisec, Constantes.ConfiguracionGeneral.ConsultaDataAgroVisec.DummyDatosCupoSapRespose);
            CupoSapTerceroDto xmlResponse = null;
            
            if (!string.IsNullOrEmpty(configuracionGeneral.Valor) && TryDeserializarXml<CupoSapTerceroDto>(configuracionGeneral.Valor, out var response))
            {
                xmlResponse = response;
            }
           
            return xmlResponse;
        }

        public Task<CupoSapTerceroDto> DatosCupoSapAsync(string cupoSap)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap GrabarCampaniaActual(CampaniaActual oRiesgos)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> GrabarCampaniaActualAsync(CampaniaActual oRiesgos)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap GrabarRiesgoComercial(RiesgoComercial oRiesgos)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> GrabarRiesgoComercialAsync(RiesgoComercial oRiesgos)
        {
            throw new NotImplementedException();
        }

        public ApoderadoSapDto[] ListarApoderadosPorProveedor(string cuit)
        {
            throw new NotImplementedException();
        }

        public Task<ApoderadoSapDto[]> ListarApoderadosPorProveedorAsync(string cuit)
        {
            throw new NotImplementedException();
        }

        public ResultadoSap Ping()
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoSap> PingAsync()
        {
            throw new NotImplementedException();
        }

        public bool ProveedorApocrifo(string cuit)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProveedorApocrifoAsync(string cuit)
        {
            throw new NotImplementedException();
        }

        public decimal TraerTipoDeCambio(DateTime? fecha, string moneda, string typeOfRate)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> TraerTipoDeCambioAsync(DateTime? fecha, string moneda, string typeOfRate)
        {
            throw new NotImplementedException();
        }

        public ResultadoValidarProveedorComercial ValidarProveedorComercial(string cuit, bool? corredor)
        {
            throw new NotImplementedException();
        }

        public Task<ResultadoValidarProveedorComercial> ValidarProveedorComercialAsync(string cuit, bool? corredor)
        {
            throw new NotImplementedException();
        }

        private bool TryDeserializarXml<T>(string xml, out T resultado) where T : class
        {
            resultado = null;

            if (string.IsNullOrWhiteSpace(xml))
                return false;

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xml))
                {
                    resultado = serializer.Deserialize(reader) as T;
                    return resultado != null;
                }
            }
            catch
            {
                // No lanzar excepción, solo indicar fallo
                return false;
            }
        }

    }
}
