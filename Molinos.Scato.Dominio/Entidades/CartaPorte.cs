using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CartaPorte : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string NroCartaPorte { get; set; }
        [Required]
        public virtual string CEE { get; set; }
        [Required]
        public virtual DateTime FechaCP { get; set; }
        
        public virtual DateTime FechaVto { get; set; }
        [Required]
        public virtual DateTime FechaEmision { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }
        public virtual OrigenVehiculo OrigenVehiculo { get; set; }
        public virtual string CTG { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual Material Material { get; set; }
        public virtual Localidad Procedencia { get; set; }
        public virtual Proveedor TitularCartaPorte { get; set; }
        public virtual Proveedor Intermediario { get; set; }
        public virtual Proveedor RtteComercial { get; set; }
        public virtual Proveedor Corredor { get; set; }
        public virtual Entregador Entregador { get; set; }
        public virtual Proveedor AgenteCompras { get; set; }
        public virtual Proveedor Destinatario { get; set; }
        public virtual Cliente DestinatarioCliente { get; set; }
        public virtual Centro CentroDestino { get; set; }
        public virtual Cliente ClienteDestino { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual int? KmRecorrer { get; set; }
        public virtual decimal? TarifaTonelada { get; set; }
        public virtual decimal? TarifaReferencia { get; set; }
        public virtual string Variedad { get; set; }
        public virtual string CodEstab { get; set; }
        public virtual Boolean? FletePagado { get; set; }
        public virtual Boolean? FleteAPagar { get; set; }
        public virtual Boolean Aparceria { get; set; }
        public virtual Boolean Desvio { get; set; }
        public virtual Boolean? EsExtranjero { get; set; }
        public virtual string AcuerdoMarco { get; set; }
        [Required]
        public virtual string Cosecha { get; set; }
        public virtual int? Caratula { get; set; }
        public virtual Proveedor Prestador { get; set; }
        public virtual BocaDestino BocaDestino { get; set; }
        [InverseProperty("CartaPorte")]
        public virtual ICollection<Vehiculo> Vehiculos { get; set; }
        public virtual string CodigoAnexo { get; set; }
        public virtual Tecnologia Tecnologia { get; set; }
        public virtual string Cupo { get; set; }
        public virtual string NumeroAduana { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual Proveedor CorredorVendedor { get; set; }
        public virtual Proveedor IntermediarioFlete { get; set; }
        public virtual Boolean? TrigoEspecial { get; set; }
        public virtual string FotoRutaDestino { get; set; }
        public virtual string FotoRutaDestinoDetalle { get; set; }
        public virtual bool? Cpe { get; set; }
        public virtual Proveedor RtteComercialVentaSecundaria { get; set; }
        public virtual Proveedor RtteComercialProductor { get; set; }
        public virtual Proveedor CorredorVendedorSecundario { get; set; }
        public virtual int? Sucursal { get; set; }
        public virtual Proveedor RtteComercialVentaSecundaria2 { get; set; }
        public virtual string Observacion { get; set; }
        public virtual long? NumeroOperativo { get; set; }
        public virtual RamalFerroviario RamalFerroviario { get; set; }
        public virtual string NumeroPrecinto { get; set; }
        public virtual Transportista TransportistaTramo2 { get; set; }
        public virtual Proveedor PagadorFlete { get; set; }
        public virtual Entregador RepresentanteRecibidor { get; set; }
        public virtual string FotoRutaSustentable { get; set; }
        public string CodigoRENSPA { get; set; }
    }
}
