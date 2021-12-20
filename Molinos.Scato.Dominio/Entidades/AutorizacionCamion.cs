using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutorizacionCamion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        
        public virtual DateTime Fecha { get; set; }
        [Required]
       
        public virtual string Comentario { get; set; }
        [Required]
        public virtual InhabilitacionCamion InhabilitacionCamion { get; set; }
        [Required]
        public virtual string NombreUsuarioResponsable { get; set; }
       
    }
}
