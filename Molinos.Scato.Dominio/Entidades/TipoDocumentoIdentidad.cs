using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoDocumentoIdentidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual string DescripcionCorta { get; set; }
        [Required]
        public virtual string CodigoSap { get; set; }
    }
}
