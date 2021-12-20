using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Localidad
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoAfip { get; set; }
        
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual Provincia Provincia { get; set; }
    }
}
