using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Motivo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual string DescripcionCorta { get; set; }
    }
}
