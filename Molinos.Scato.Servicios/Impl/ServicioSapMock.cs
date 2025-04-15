using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Ninject.Extensions.Logging;
using NPOI.SS.Formula.Functions;


namespace Molinos.Scato.Servicios
{
    internal class ServicioSapMock : ZSDWS_SCATO
    {

        protected IRepositorio Repositorio { get; private set; }
        protected IConversor Conversor { get; private set; }
        protected ILogger Logger { get; private set; }

        public ServicioSapMock(IRepositorio repositorio, IConversor conversor , ILogger logger)
        {
            Repositorio = repositorio;
            Conversor = conversor;
            Logger = logger;
        }

        public AnulaContabilizacionResponse1 AnulaContabilizacion(AnulaContabilizacionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AnulaContabilizacionResponse1> AnulaContabilizacionAsync(AnulaContabilizacionRequest request)
        {
            throw new NotImplementedException();
        }

        public ConsultaLoteDispYRecepcionResponse1 ConsultaLoteDispYRecepcion(ConsultaLoteDispYRecepcionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultaLoteDispYRecepcionResponse1> ConsultaLoteDispYRecepcionAsync(ConsultaLoteDispYRecepcionRequest request)
        {
            throw new NotImplementedException();
        }

        public ConsultaOrdenDeCargaResponse1 ConsultaOrdenDeCarga(ConsultaOrdenDeCargaRequest request)
        {
            Logger.Info("Inicia ConsultaOrdenDeCarga Dummy");
            var confiFasDummy = Repositorio.Obtener<Dominio.Entidades.ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.ServicioSap && x.Nombre == Constantes.ConfiguracionGeneral.Servicios.SapDummyResponse && x.CentroId == null);
            var xml = new ConsultaOrdenDeCargaResponse1();
            xml.ConsultaOrdenDeCargaResponse = new ConsultaOrdenDeCargaResponse();

            if (string.IsNullOrEmpty(confiFasDummy.Valor))
            {
                Logger.Info("No se encontró la configuración para el Dummy de SAP");    
                return xml;
            }

            byte[] bytes = Convert.FromBase64String(confiFasDummy.Valor);
            string xmlMock = Encoding.UTF8.GetString(bytes);

            // Comprobar si el BOM está presente y eliminarlo si lo está
            if (xmlMock.StartsWith("\uFEFF"))
            {
                xmlMock = xmlMock.Substring(1);  // Eliminar el primer carácter (BOM)
            }

            xmlMock = Regex.Replace(xmlMock, @"\s+", "");

            Logger.Info("Xml Mock: {0}", xmlMock);
            var salida = DeserializarXml<ZSDES0300[]>(xmlMock);
           
            xml.ConsultaOrdenDeCargaResponse.Salida = salida;

            return xml;
        }

        public Task<ConsultaOrdenDeCargaResponse1> ConsultaOrdenDeCargaAsync(ConsultaOrdenDeCargaRequest request)
        {
            throw new NotImplementedException();
        }

        public ConsultaPedidoResponse1 ConsultaPedido(ConsultaPedidoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultaPedidoResponse1> ConsultaPedidoAsync(ConsultaPedidoRequest request)
        {
            throw new NotImplementedException();
        }

        public ContabilizarIngresosResponse1 ContabilizarIngresos(ContabilizarIngresosRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ContabilizarIngresosResponse1> ContabilizarIngresosAsync(ContabilizarIngresosRequest request)
        {
            throw new NotImplementedException();
        }

        public DatosClienteResponse1 DatosCliente(DatosClienteRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DatosClienteResponse1> DatosClienteAsync(DatosClienteRequest request)
        {
            throw new NotImplementedException();
        }

        public DatosMaterialesResponse1 DatosMateriales(DatosMaterialesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DatosMaterialesResponse1> DatosMaterialesAsync(DatosMaterialesRequest request)
        {
            throw new NotImplementedException();
        }

        public DatosProveedoresResponse1 DatosProveedores(DatosProveedoresRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DatosProveedoresResponse1> DatosProveedoresAsync(DatosProveedoresRequest request)
        {
            throw new NotImplementedException();
        }

        public EgresoSinFleteFazonesResponse1 EgresoSinFleteFazones(EgresoSinFleteFazonesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<EgresoSinFleteFazonesResponse1> EgresoSinFleteFazonesAsync(EgresoSinFleteFazonesRequest request)
        {
            throw new NotImplementedException();
        }

        public EgresosNoProductivosResponse1 EgresosNoProductivos(EgresosNoProductivosRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<EgresosNoProductivosResponse1> EgresosNoProductivosAsync(EgresosNoProductivosRequest request)
        {
            throw new NotImplementedException();
        }

        public Fill_Z1000Response1 Fill_Z1000(Fill_Z1000Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Fill_Z1000Response1> Fill_Z1000Async(Fill_Z1000Request request)
        {
            throw new NotImplementedException();
        }

        public FletesDobleTramoResponse1 FletesDobleTramo(FletesDobleTramoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<FletesDobleTramoResponse1> FletesDobleTramoAsync(FletesDobleTramoRequest request)
        {
            throw new NotImplementedException();
        }

        public IngresosBodegaResponse1 IngresosBodega(IngresosBodegaRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IngresosBodegaResponse1> IngresosBodegaAsync(IngresosBodegaRequest request)
        {
            throw new NotImplementedException();
        }

        public IngresosEgresosFazonesResponse1 IngresosEgresosFazones(IngresosEgresosFazonesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IngresosEgresosFazonesResponse1> IngresosEgresosFazonesAsync(IngresosEgresosFazonesRequest request)
        {
            throw new NotImplementedException();
        }

        public Mov305Response1 Mov305(Mov305Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Mov305Response1> Mov305Async(Mov305Request request)
        {
            throw new NotImplementedException();
        }

        public Mov975Response1 Mov975(Mov975Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Mov975Response1> Mov975Async(Mov975Request request)
        {
            throw new NotImplementedException();
        }

        public MovAjusteResponse1 MovAjuste(MovAjusteRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MovAjusteResponse1> MovAjusteAsync(MovAjusteRequest request)
        {
            throw new NotImplementedException();
        }

        public PesaBrutoResponse1 PesaBruto(PesaBrutoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PesaBrutoResponse1> PesaBrutoAsync(PesaBrutoRequest request)
        {
            throw new NotImplementedException();
        }

        public PesaNetoResponse1 PesaNeto(PesaNetoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PesaNetoResponse1> PesaNetoAsync(PesaNetoRequest request)
        {
            throw new NotImplementedException();
        }

        public ValidacionCOTResponse1 ValidacionCOT(ValidacionCOTRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ValidacionCOTResponse1> ValidacionCOTAsync(ValidacionCOTRequest request)
        {
            throw new NotImplementedException();
        }

        public ValidaContratosResponse1 ValidaContratos(ValidaContratosRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ValidaContratosResponse1> ValidaContratosAsync(ValidaContratosRequest request)
        {
            throw new NotImplementedException();
        }

        public ValidaStockYLoteResponse1 ValidaStockYLote(ValidaStockYLoteRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ValidaStockYLoteResponse1> ValidaStockYLoteAsync(ValidaStockYLoteRequest request)
        {
            throw new NotImplementedException();
        }

        public VerifPedTrasladoRedespachoResponse1 VerifPedTrasladoRedespacho(VerifPedTrasladoRedespachoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<VerifPedTrasladoRedespachoResponse1> VerifPedTrasladoRedespachoAsync(VerifPedTrasladoRedespachoRequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_CONS_PP_TAB_CUB_TANResponse1 Z_SDMF_RFC_CONS_PP_TAB_CUB_TAN(Z_SDMF_RFC_CONS_PP_TAB_CUB_TANRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_CONS_PP_TAB_CUB_TANResponse1> Z_SDMF_RFC_CONS_PP_TAB_CUB_TANAsync(Z_SDMF_RFC_CONS_PP_TAB_CUB_TANRequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_CREDITOSResponse1 Z_SDMF_RFC_CREDITOS(Z_SDMF_RFC_CREDITOSRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_CREDITOSResponse1> Z_SDMF_RFC_CREDITOSAsync(Z_SDMF_RFC_CREDITOSRequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_MOV_311Response1 Z_SDMF_RFC_MOV_311(Z_SDMF_RFC_MOV_311Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_MOV_311Response1> Z_SDMF_RFC_MOV_311Async(Z_SDMF_RFC_MOV_311Request request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_MSJ_ENTREGAResponse1 Z_SDMF_RFC_MSJ_ENTREGA(Z_SDMF_RFC_MSJ_ENTREGARequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_MSJ_ENTREGAResponse1> Z_SDMF_RFC_MSJ_ENTREGAAsync(Z_SDMF_RFC_MSJ_ENTREGARequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse1 Z_SDMF_RFC_VENTA_TRIGO_MAIZ(Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_VENTA_TRIGO_MAIZResponse1> Z_SDMF_RFC_VENTA_TRIGO_MAIZAsync(Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_Z2100Response1 Z_SDMF_RFC_Z2100(Z_SDMF_RFC_Z2100Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_Z2100Response1> Z_SDMF_RFC_Z2100Async(Z_SDMF_RFC_Z2100Request request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_Z2100TResponse1 Z_SDMF_RFC_Z2100T(Z_SDMF_RFC_Z2100TRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_Z2100TResponse1> Z_SDMF_RFC_Z2100TAsync(Z_SDMF_RFC_Z2100TRequest request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_RFC_ZE7550Response1 Z_SDMF_RFC_ZE7550(Z_SDMF_RFC_ZE7550Request request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_RFC_ZE7550Response1> Z_SDMF_RFC_ZE7550Async(Z_SDMF_RFC_ZE7550Request request)
        {
            throw new NotImplementedException();
        }

        public Z_SDMF_Z2200NResponse1 Z_SDMF_Z2200N(Z_SDMF_Z2200NRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Z_SDMF_Z2200NResponse1> Z_SDMF_Z2200NAsync(Z_SDMF_Z2200NRequest request)
        {
            throw new NotImplementedException();
        }

        private T DeserializarXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
