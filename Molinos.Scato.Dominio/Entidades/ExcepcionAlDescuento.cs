using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ExcepcionAlDescuento : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Proveedor Proveedor { get; set; }
        [Required]
        public virtual DateTime FechaDesde { get; set; }
        [Required]
        public virtual DateTime FechaHasta { get; set; }
        [Required]
        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
        [Required]
        public virtual Camara Camara { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string Usuario { get; set; }
    }
}
