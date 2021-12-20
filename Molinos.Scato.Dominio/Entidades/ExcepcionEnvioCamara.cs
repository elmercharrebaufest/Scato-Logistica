using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ExcepcionEnvioCamara : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        [Required]
        public virtual CaracteristicaDeCalidad CaracteristicaMaterial { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Proveedor Proveedor { get; set; }
        [Required]
        public virtual Entregador Entregador { get; set; }
    }
}
