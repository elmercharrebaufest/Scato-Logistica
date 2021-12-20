using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Salida : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Codigo { get; set; }

        [Required]
        public virtual string Descripcion { get; set; }
    }
}
