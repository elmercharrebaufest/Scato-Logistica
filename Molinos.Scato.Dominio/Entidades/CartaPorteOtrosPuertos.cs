using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CartaPorteOtrosPuertos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual DateTime FechaCreacion { get; set; }
        [Required]
        public virtual string UsuarioCreacion { get; set; }
        public virtual DateTime? FechaUltimaModificacion { get; set; }
        public virtual string UsuarioUltimaModificacion { get; set; }
        public virtual int? DocumentoExterno_Id { get; set; }
        [Required]
        public virtual string NroCartaPorte { get; set; }
        public virtual DateTime? FechaCarga { get; set; }
        public virtual DateTime? FechaVencimiento { get; set; }
        public virtual DateTime? FechaArribo { get; set; }
        public virtual DateTime? FechaDescarga { get; set; }
        public virtual string CEE { get; set; }
        public virtual string CTG { get; set; }
        public virtual string Establecimiento { get; set; }
        public virtual string Planta { get; set; }
        public virtual string TitularCuit { get; set; }
        public virtual string TitularRazonSocial { get; set; }
        public virtual string IntermediarioCuit { get; set; }
        public virtual string IntermediarioRazonSocial { get; set; }
        public virtual string RemitenteCuit { get; set; }
        public virtual string RemitenteRazonSocial { get; set; }
        public virtual string ProductoCodigo { get; set; }
        public virtual string ProductoDescripcion { get; set; }
        public virtual string ProductoCosecha { get; set; }
        public virtual int? Material_Id { get; set; }
        public virtual string ProcedenciaCodigo { get; set; }
        public virtual string ProcedenciaLocalidad { get; set; }
        public virtual string ProcedenciaPB { get; set; }
        public virtual string ProcedenciaPN { get; set; }
        public virtual string ProcedenciaPT { get; set; }
        public virtual string ProcedenciaProvincia { get; set; }
        public virtual string ProcedenciaCP { get; set; }
        public virtual string CorredorCuit { get; set; }
        public virtual string CorredorRazonSocial { get; set; }
        public virtual string EntregadorCuit { get; set; } //Representante Enregador
        public virtual string EntregadorRazonSocial { get; set; }
        public virtual string DestinatarioCuit { get; set; }
        public virtual string DestinatarioRazonSocial { get; set; }
        public virtual string DestinoCuit { get; set; }
        public virtual string DestinoRazonSocial { get; set; }
        public virtual string DestinoPB { get; set; }
        public virtual string DestinoPN { get; set; }
        public virtual string DestinoPT { get; set; }
        public virtual string TransporteCuit { get; set; }
        public virtual string TransporteRazonSocial { get; set; }
        public virtual string TransportePatente { get; set; }
        public virtual string ChoferCuit { get; set; }
        public virtual string ChoferRazonSocial { get; set; }
        public virtual string MermaTotal { get; set; }
        public virtual string NetoConvenido { get; set; }
        public virtual string DescuentosKgsHumedad {get;set;}
        public virtual string DescuentosKgsCalidad { get; set; }
        public virtual string DescuentosKgsSecada { get; set; }
        [InverseProperty("CartaPorteOtrosPuertos")]
        public virtual ICollection<CaracteristicasCartaPorteOtrosPuertos> Caracteristicas { get; set; }

        //Campos Electronica
        public virtual int? TipoCartaPorte { get; set; }
        public virtual int? Sucursal { get; set; }
        public virtual long? NroOrden { get; set; }
        public virtual string Estado { get; set; }
        public virtual string Domicilio { get; set; }
        public virtual int? PlantaOrigen { get; set; }
        public virtual bool? RetiroProductor { get; set; }
        public virtual long? CertificadoCOE { get; set; }
        //public virtual Proveedor RtteComercialProductor { get; set; } RemitenteCuit
        public virtual string CuitRemitenteComercialVentaPrimaria { get; set; }
        public virtual string RazonSocialRemitenteComercialVentaPrimaria { get; set; }
        public virtual string CuitRemitenteComercialVentaSecundaria { get; set; }
        public virtual string RazonSocialRemitenteComercialVentaSecundaria { get; set; }
        public virtual string CuitMercadoATermino { get; set; }
        public virtual string RazonSocialMercadoATermino { get; set; }
        public virtual string CuitCorredorVentaSecundaria { get; set; }
        public virtual string RazonSocialCorredorVentaSecundaria { get; set; }
        public virtual int? PlantaDestino { get; set; }
        public virtual int? KmRecorrer { get; set; }
        public virtual string Cupo { get; set; }
        public virtual double Tarifa { get; set; }
        public virtual string CuitPagadorFlete { get; set; }
        public virtual string RazonSocialPagadorFlete { get; set; }
        public virtual bool? MercaderiaFumigada { get; set; }
        public virtual string CuitRepresentanteRecibidor { get; set; }
        public virtual string RazonSocialRepresentanteRecibidor { get; set; }
        public virtual string CuitOrigen { get; set; }
        public virtual string Observacion { get; set; }
        public virtual DateTime? FechaUltimaActualizacion { get; set; }
        public virtual long? NroOperativo { get; set; }
        public virtual string CuitRemitenteComercialVentaSecundaria2 { get; set; }
        public virtual string RazonSocialRemitenteComercialVentaSecundaria2 { get; set; }
        public virtual int? RamalFerroviario { get; set; }
        public virtual string NumeroPrecinto { get; set; }
        public virtual byte[] Pdf { get; set; }
        public virtual bool EsSustentable { get; set; }
        public virtual string CodigoEstablecimientoSustentable { get; set; }
    }
}
