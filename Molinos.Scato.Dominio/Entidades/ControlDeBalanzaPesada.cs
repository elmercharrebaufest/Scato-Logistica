using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ControlDeBalanzaPesada : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ControlDeBalanza ControlDeBalanza { get; set; }
        public virtual Balanza Balanza { get; set; }
        [Required]
        public virtual int Peso { get; set; }
        public virtual DateTime Fecha { get; set; }
    }
}