using System;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpImpresion")]
    public class ImpImpresionGenerica : Impresion
    {
        public virtual string CTG { get; set; }
        public virtual DateTime FechaEmision { get; set; }
        public virtual DateTime FechaCP { get; set; }
        public virtual DateTime FechaVencimiento { get; set; }
        public virtual string TitularCP { get; set; }
        public virtual string CuitTitularCP { get; set; }
        public virtual string Intermediario { get; set; }
        public virtual string CuitIntermediario { get; set; }
        public virtual string RtteComercial { get; set; }
        public virtual string CuitRtteComercial { get; set; }
        public virtual string Corredor { get; set; }
        public virtual string CuitCorredor { get; set; }
        public virtual string Entregador { get; set; }
        public virtual string CuitEntregador { get; set; }
        public virtual string Destinatario { get; set; }
        public virtual string CuitDestinatario { get; set; }
        public virtual string Destino { get; set; }
        public virtual string CuitDestino { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string CuitTransportista { get; set; }
        public virtual string Chofer { get; set; }
        public virtual string CuitChofer { get; set; }
        public virtual string Material { get; set; }
        public virtual string MaterialDescCorta { get; set; }
        public virtual string MaterialCodigoONCCA { get; set; }
        public virtual string Variedad { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual string CodigoEstablecimiento { get; set; }
        public virtual string PesoBrutoOrigen { get; set; }
        public virtual string PesoTaraOrigen { get; set; }
        public virtual string PesoNetoOrigen { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string KmARecorrer { get; set; }
        public virtual string TarifaReferencia { get; set; }
        public virtual string TarifaTonelada { get; set; }
        public virtual string CodigoAnexo { get; set; }
        public virtual string Prestador { get; set; }
        public virtual string CuitPrestador { get; set; }
        public virtual string DomicilioBocaDestino { get; set; }
        public virtual string ProvinciaBocaDestino { get; set; }
        public virtual string LocalidadBocaDestino { get; set; }
        public virtual string BocaDestino { get; set; }
        public virtual string AcuerdoMarco { get; set; }
        public virtual string Caratula { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string PesoNetoSinHumedad { get; set; }
        public virtual string PesoNetoIngreso { get; set; }
        public virtual string PesoNetoEgreso { get; set; }
        public virtual string LocalidadCentroOrigen { get; set; }
        public virtual string ProvinciaCentroOrigen { get; set; }
        public virtual string CodigoPostalCentroOrigen { get; set; }
        public virtual string DireccionCentroOrigen { get; set; }
        public virtual string LocalidadCentroDestino { get; set; }
        public virtual string ProvinciaCentroDestino { get; set; }
        public virtual string DireccionClienteDestino { get; set; }
        public virtual string LocalidadClienteDestino { get; set; }
        public virtual string ProvinciaClienteDestino { get; set; }
        public virtual string CodigoPostalCentroDestino { get; set; }
        public virtual string DireccionCentroDestino { get; set; }
        public virtual string TipoDocumentoIngreso { get; set; }
        public virtual string TipoDeComprobanteONCCA { get; set; }
        public virtual string NumeroDeDocumentoDeIngreso { get; set; }
        public virtual string SaldosSTOCK { get; set; }
        public virtual string ObservacionesONCCA { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual string FletePagado { get; set; }
        public virtual string FleteAPagar { get; set; }
        public virtual string TextoLibre { get; set; }

        public virtual string NumeroCiu { get; set; }
        public virtual string INVBodega { get; set; }
        public virtual string CuitBodega { get; set; }
        public virtual string IIBBBodega { get; set; }
        public virtual string RazonSocialVinatero { get; set; }
        public virtual string INVVinatero { get; set; }
        public virtual string CuitViñatero { get; set; }
        public virtual string IIBBViñatero { get; set; }
        public virtual string EsCamion { get; set; }
        public virtual string EsAcoplado { get; set; }
        public virtual string EsBines { get; set; }
        public virtual string EsMoliendaEnVinedos { get; set; }
        public virtual string EsTractor { get; set; }
        public virtual string MarcaVehiculo { get; set; }
        public virtual string ModeloVehiculo { get; set; }
        public virtual string INVVariedad { get; set; }
        public virtual string TenorAzucarino { get; set; }
        public virtual string EsUvaPropia { get; set; }
        public virtual string EsUvaTerceros { get; set; }
        public virtual string RazonSocialBodega { get; set; }
        public virtual string FechaPesoNetoBodega { get; set; }
        public virtual TipoDeWorkflow TipoDeWorkflow { get; set; }

        public virtual string CorredorVendedor { get; set; }
        public virtual string CuitCorredorVendedor { get; set; }
        public virtual string IntermediarioFlete { get; set; }
        public virtual string MercadoATermino { get; set; }
        public virtual string CuitMercadoATermino { get; set; }
        public virtual string CuitIntermediarioDelFlete { get; set; }
    }
}
