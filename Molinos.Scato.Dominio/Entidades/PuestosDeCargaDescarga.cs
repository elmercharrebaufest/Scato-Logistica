using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PuestosDeCargaDescarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Nombre { get; set; }
        [Required]
        public virtual string Codigo { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual Centro Centro { get; set; }

        public virtual ICollection<AsignacionDeRecorrido> AsignacionesDeRecorrido { get; set; }
        public virtual bool EsSojaSustentable { get; set; }
    }
}
