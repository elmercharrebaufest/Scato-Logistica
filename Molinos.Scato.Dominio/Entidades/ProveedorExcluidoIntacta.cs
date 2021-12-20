using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ProveedorExcluidoIntacta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
