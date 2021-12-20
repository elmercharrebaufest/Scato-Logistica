using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TarjetaRango : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Codigo { get; set; }
        [Required]
        public virtual DateTime ValidoDesde { get; set; }
        [Required]
        public virtual DateTime ValidoHasta { get; set; }
        [Required]
        public virtual string RangoDesde { get; set; }
        [Required]
        public virtual string RangoHasta { get; set; }
        [Required]
        public virtual Centro Centro { get; set; }
    }
}
