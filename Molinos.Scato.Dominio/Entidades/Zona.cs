using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Zona
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
    }
}
