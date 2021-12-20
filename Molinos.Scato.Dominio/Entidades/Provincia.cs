using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Provincia
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int CodigoAfip { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual string DescripcionCollate_CI_AS { get; set; }
        [Required]
        public virtual Pais Pais { get; set; }
    }
}
