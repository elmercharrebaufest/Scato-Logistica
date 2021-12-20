using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RamalFerroviario
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual int CodigoAfip { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual DateTime FechaCreacion { get; set; }
        [Required]
        public virtual bool Deshabilitada { get; set; }
    }
}
