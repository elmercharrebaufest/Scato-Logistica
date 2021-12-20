using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [KnownType(typeof(FormatoDeImpresionDto))]
    [KnownType(typeof(TipoImpresion))]
    [KnownType(typeof(FirmaDto))]

    [KnownType(typeof(ImpAsignacionDeRutaDto))]
    [KnownType(typeof(ImpCertificadoDeAnalisisDto))]
    [KnownType(typeof(ImpAsigRecorrCtrolCalidDto))]
    [KnownType(typeof(ImpCertificadoDeCartaPorteDto))]
    [KnownType(typeof(ImpConstanciaDeEntregaLaserDto))]
    [KnownType(typeof(ImpDeclaracionFosfinaDto))]
    [KnownType(typeof(ImpDocumentoDeEntradaDto))]
    [KnownType(typeof(ImpFormulario239Dto))]
    [KnownType(typeof(ImpIdentificacionEnvioLoteACamaraDto))]
    [KnownType(typeof(ImpIdentificacionMicromuestraDto))]
    [KnownType(typeof(ImpIdentificacionMuestraCaladoDto))]
    [KnownType(typeof(ImpSolicitudDeAnalisisDto))]
    [KnownType(typeof(ImpTicketPesadaDto))]
    [KnownType(typeof(ImpImpresionGenericaDto))]
    [KnownType(typeof(ImpInformeDeRecepcionItemDto))]
    [KnownType(typeof(ImpReciboMunicipalDto))]
    [KnownType(typeof(ImpIdentificacionMuestraAuditoriaDto))]
    [KnownType(typeof(ImpEtiquetaAuditoriaDto))]
    [KnownType(typeof(ImpEtiquetaIntactaDto))]
    [KnownType(typeof(ImpTicketPesadaBodegaDto))]
    [KnownType(typeof(ImpTicketPesadaAduanaDto))]
    [KnownType(typeof(ImpEtiquetaRubrosAnalizarDto))]
    [KnownType(typeof(ImpReciboMunicipalImportacionDto))]
    [KnownType(typeof(ImpCartaPorteUrenportDto))]
    [KnownType(typeof(ImpGaritaSalidaDto))] 
    [KnownType(typeof(ImpResumenHojaDeRutaDto))] 
    [KnownType(typeof(ImpEtiquetaAuditoriaCamaraDto))]
    public class ImprimirDocumento : Comando
    {
        public int Id { get; set; }
        public int Impresora { get; set; }
        public int CentroId { get; set; }
        public int CantCopias { get; set; }
        public object Dto { get; set; }
        public string Direccion { get; set; }
        public FormatoDeImpresionDto Formato { get; set; }
        public TipoImpresion TipoImpresion { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
