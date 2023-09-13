using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class InhabilitacionChofer : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual DateTime FechaDesde { get; set; }
        [Required]
        public virtual DateTime FechaHasta { get; set; }
        [Required]
        public virtual string Motivo { get; set; }
  
        public virtual Centro Centro { get; set; }
        [Required]
        public virtual string NombreUsuarioResponsable { get; set; }

        [InverseProperty("InhabilitacionChofer")]
        public virtual ICollection<Adjunto> Adjuntos { get; set; }

    }
}
