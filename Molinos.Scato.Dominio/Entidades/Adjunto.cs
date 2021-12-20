using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Adjunto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Archivo { get; set; }

        public virtual String Descripcion { get; set; }

        [InverseProperty("Adjuntos")] 
        public virtual InhabilitacionChofer InhabilitacionChofer { get; set; }

        [InverseProperty("Adjuntos")]
        public virtual InhabilitacionCamion InhabilitacionCamion { get; set; }

    }
}
