using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaPorteOtrosPuertosDto
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public string UsuarioUltimaModificacion { get; set; }
        public int? DocumentoExternoId { get; set; }
        public string NumeroCartaPorte { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaArribo { get; set; }
        public DateTime FechaDescarga { get; set; }
        public string CEE { get; set; }
        public string CTG { get; set; }
        public string Establecimiento { get; set; }
        public string Planta { get; set; }
        public string TitularCuit { get; set; }
        public string TitularRazonSocial { get; set; }
        public string IntermediarioCuit { get; set; }
        public string IntermediarioRazonSocial { get; set; }
        public string RemitenteCuit { get; set; }
        public string RemitenteRazonSocial { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoDescripcion { get; set; }
        public string ProductoCosecha { get; set; }
        public int MaterialId { get; set; }
        public string ProcedenciaCodigo { get; set; }
        public string ProcedenciaLocalidad { get; set; }
        public string ProcedenciaPB { get; set; }
        public string ProcedenciaPN { get; set; }
        public string ProcedenciaPT { get; set; }
        public string ProcedenciaProvincia { get; set; }
        public string ProcedenciaCP { get; set; }
        public string CorredorCuit { get; set; }
        public string CorredorRazonSocial { get; set; }
        public string EntregadorCuit { get; set; }
        public string EntregadorRazonSocial { get; set; }
        public string DestinatarioCuit { get; set; }
        public string DestinatarioRazonSocial { get; set; }
        public string DestinoCuit { get; set; }
        public string DestinoRazonSocial { get; set; }
        public string DestinoPB { get; set; }
        public string DestinoPN { get; set; }
        public string DestinoPT { get; set; }
        public string TransporteCuit { get; set; }
        public string TransporteRazonSocial { get; set; }
        public string TrasportePatente { get; set; }
        public string ChoferCuit { get; set; }
        public string ChoferRazonSocial { get; set; }
        public string MermaTotal { get; set; }
        public string NetoConvenido { get; set; }
        public string DescuentosKgsHumedad { get; set; }
        public string DescuentosKgsCalidad { get; set; }
        public string DescuentosKgsSecada { get; set; }
        public string FotoCpBase64 { get; set; }
        public string FotoCpNombreArchivo { get; set; }
        public List<CaracteristicasCartaPorteOtrosPuertosDto> Caracteristicas { get; set; }
        //CPE
        public int? TipoCartaPorte { get; set; }
        public int? Sucursal { get; set; }
        public long? NroOrden { get; set; }
        public string Estado { get; set; }
        public string Domicilio { get; set; }
        public int? PlantaOrigen { get; set; }
        public bool? RetiroProductor { get; set; }
        public long? CertificadoCOE { get; set; }
        //publil Proveedor RtteComercialProductor { get; set; } RemitenteCuit
        public string CuitRemitenteComercialVentaPrimaria { get; set; }
        public string RazonSocialRemitenteComercialVentaPrimaria { get; set; }
        public string CuitRemitenteComercialVentaSecundaria { get; set; }
        public string RazonSocialRemitenteComercialVentaSecundaria { get; set; }
        public string CuitMercadoATermino { get; set; }
        public string RazonSocialMercadoATermino { get; set; }
        public string CuitCorredorVentaSecundaria { get; set; }
        public string RazonSocialCorredorVentaSecundaria { get; set; }
        public int? PlantaDestino { get; set; }
        public int? KmRecorrer { get; set; }
        public string Cupo { get; set; }
        public double Tarifa { get; set; }
        public string CuitPagadorFlete { get; set; }
        public string RazonSocialPagadorFlete { get; set; }
        public bool? MercaderiaFumigada { get; set; }
        public string CuitRepresentanteRecibidor { get; set; }
        public string RazonSocialRepresentanteRecibidor { get; set; }
        public string CuitOrigen { get; set; }
        public string Observacion { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public long? NroOperativo { get; set; }
        public string CuitRemitenteComercialVentaSecundaria2 { get; set; }
        public string RazonSocialRemitenteComercialVentaSecundaria2 { get; set; }
        public int? RamalFerroviario { get; set; }
        public string NumeroPrecinto { get; set; }
        public byte[] Pdf { get; set; }
    }
}
