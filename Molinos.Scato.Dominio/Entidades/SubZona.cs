using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class SubZona
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual Zona Zona { get; set; }
    }
}
