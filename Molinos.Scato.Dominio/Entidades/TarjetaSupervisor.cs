using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TarjetaSupervisor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual int Salida { get; set; }
        public virtual Centro Centro { get; set; }
        [InverseProperty("TarjetasSupervisorAsociadas")]
        [Required]
        public virtual IList<PuestoDeTrabajo> PuestosDeTrabajoAsociados { get; set; }
    }
}
