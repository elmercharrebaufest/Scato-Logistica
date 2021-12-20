using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TarjetaBloqueada : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual string Motivo { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
