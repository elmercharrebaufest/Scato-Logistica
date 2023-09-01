using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ExcepcionAlControlProveedor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Material Material { get; set; }
        [Required]
        public virtual DateTime FechaDesde { get; set; }
        [Required]
        public virtual DateTime FechaHasta { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual TipoDestino TipoDestino { get; set; }
        public virtual Centro CentroDestino { get; set; }
        public virtual Cliente ClienteDestino { get; set; }
        public virtual DateTime FechaDeCarga { get; set; }
        public virtual string Usuario { get; set; }
        public virtual MotivoExcepcionAlControl Motivo { get; set; }
        public virtual Proveedor ProveedorDestino { get; set; }
    }
}
