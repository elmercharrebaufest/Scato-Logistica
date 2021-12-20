using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("Bodega")]
    public class Bodega : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
    }
}
