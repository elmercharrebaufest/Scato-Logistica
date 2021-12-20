using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TaraContenedor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string CodigoContenedor { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual int PesoTara { get; set; }
    }
}
