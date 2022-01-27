using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RegistroInactividad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Usuario { get; set; }
        public virtual int? MotivoId { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime? FechaFinal { get; set; }
        public int? PuestoTrabajoId { get; set; }
    }
}
