using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Chofer : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Apellido { get; set; }
        [Required]
        public virtual string Nombre { get; set; }
        [Required]
        public virtual TipoDocumentoIdentidad TipoDocumentoIdentidad { get; set; }
        [Required]
        public virtual string NumeroDeDocumento { get; set; }
        [Required]
        public virtual string Cuil { get; set; }
    }
}
