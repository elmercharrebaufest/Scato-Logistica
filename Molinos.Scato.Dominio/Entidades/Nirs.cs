using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Nirs : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual string DescripcionCorta { get; set; }
        public virtual string Codigo { get; set; }
        public virtual Modalidad Modalidad { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string PuestoDeTrabajo { get; set; }
    }
}
