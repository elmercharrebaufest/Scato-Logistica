using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenDeDescargaFason : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual string NumeroRemito { get; set; }
        [Required]
        public virtual DateTime FechaOD { get; set; }
        [Required]
        public virtual Cliente Cliente { get; set; }
        [Required]
        public virtual Localidad Procedencia { get; set; }
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        public virtual int?PesoBrutoOrigen { get; set; }
        public virtual int? PesoTaraOrigen { get; set; }
        [Required]
        public virtual int PesoNetoOrigen { get; set; }

        public virtual Recorrido Recorrido { get; set; }
        public virtual bool? EsExtranjero { get; set; }

        public virtual string NumeroCTG { get; set; }
        public virtual string Sucursal { get; set; }
        public virtual string NroOrden { get; set; }
    }
}

