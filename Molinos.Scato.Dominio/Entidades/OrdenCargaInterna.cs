using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenCargaInterna: IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string NumeroOrden { get; set; }
        [Required]
        public virtual DateTime FechaEmision { get; set; }
        [Required]
        public virtual DateTime FechaCreacion { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        [Required]
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        [Required]
        public virtual Cliente Destino { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual Recorrido Recorrido { get; set; }
        public virtual Localidad LocalidadDestino { get; set; }
        public virtual string KmRecorrer { get; set; }
        public virtual bool? EsExtranjero { get; set; }
        public virtual bool DerivadoGranarioHabilitado { get; set; }
        public virtual int? PlantaDGDestino { get; set; }
        public virtual int? OrdenDomicilioDestino { get; set; }
        public virtual Cliente PagadorFlete { get; set; }
        public virtual int? TipoDomicilioDestino { get; set; }
        public virtual string Id_operaciones { get; set; }
    }
}
