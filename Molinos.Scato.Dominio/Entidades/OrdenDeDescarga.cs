using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenDeDescarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual DateTime FechaMovimiento { get; set; }
        [Required]
        public virtual Proveedor Proveedor { get; set; }
        [Required]
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }

        public virtual Recorrido Recorrido { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}

