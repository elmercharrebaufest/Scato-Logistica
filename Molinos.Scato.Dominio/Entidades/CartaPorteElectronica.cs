using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CartaPorteElectronica : IIdentificable
    {
        [Required]
        public virtual int Id { get; set; }
        public virtual int? TipoCartaPorte { get; set; }
        public virtual int? Sucursal { get; set; }
        public virtual long? NroOrden { get; set; }
        public virtual int? Planta { get; set; }
        public virtual long? NroCTG { get; set; }
        public virtual DateTime? FechaEmision { get; set; }
        public virtual string Estado { get; set; }
        public virtual DateTime? FechaCP { get; set; }
        public virtual DateTime? FechaVto { get; set; }
        public virtual int? Provincia { get; set; }
        public virtual int? Localidad { get; set; }
        public virtual string Domicilio { get; set; }
        public virtual int? PlantaOrigen { get; set; }
        public virtual bool? RetiroProductor { get; set; }
        public virtual long? CertificadoCOE { get; set; }
        public virtual long? CuitRemitenteComercialProductor { get; set; }
        public virtual long? CuitIntermediario { get; set; }
        public virtual long? CuitRemitenteComercialVentaPrimaria { get; set; }
        public virtual long? CuitRemitenteComercialVentaSecundaria { get; set; }
        public virtual long? CuitMercadoATermino { get; set; }
        public virtual long? CuitCorredorVentaPrimaria { get; set; }
        public virtual long? CuitCorredorVentaSecundaria { get; set; }
        public virtual long? CuitRepresentanteEntregador { get; set; }
        public virtual int? Material { get; set; }
        public virtual int? Cosecha { get; set; }
        public virtual int? PesoBruto { get; set; }
        public virtual int? PesoTara { get; set; }
        public virtual long? CuitDestino { get; set; }
        public virtual long? LocalidadDestino { get; set; }
        public virtual long? ProvinciaDestino { get; set; }
        public virtual int? PlantaDestino { get; set; }
        public virtual long? CuitDestinatario { get; set; }
        public virtual long? CuitTransportista { get; set; }
        public virtual string Dominio { get; set; }
        public virtual DateTime? FechaPartida { get; set; }
        public virtual int? KmRecorrer { get; set; }
        public virtual string CodigoTurno { get; set; }
        public virtual long? CuitChofer { get; set; }
        public virtual double Tarifa { get; set; }
        public virtual long? CuitPagadorFlete { get; set; }
        public virtual long? CuitIntermediarioFlete { get; set; }
        public virtual bool? MercaderiaFumigada { get; set; }
        public virtual long? CuitRepresentanteRecibidor { get; set; }
        public virtual long? CuitOrigen { get; set; }
        public virtual string Observacion { get; set; }
        public virtual DateTime? FechaUltimaActualizacion { get; set; }
        public virtual long? NroOperativo { get; set; }
        public virtual long? CuitRemitenteComercialVentaSecundaria2 { get; set; }
        public virtual int? RamalFerroviario { get; set; }
        public virtual string NumeroPrecinto { get; set; }
        public virtual byte[] Pdf { get; set; }
        public virtual long? CuitTransportistaTramo2 { get; set; }        
        public virtual double TarifaReferencia { get; set; }
        public virtual DateTime? FechaCacheado { get; set; }
    }
}
