using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutorizacionChofer : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        
        public virtual DateTime Fecha { get; set; }
        [Required]
       
        public virtual string Comentario { get; set; }
        [Required]
        public virtual InhabilitacionChofer InhabilitacionChofer { get; set; }
        [Required]
        public virtual string NombreUsuarioResponsable { get; set; }
       
    }
}
