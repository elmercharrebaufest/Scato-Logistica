using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EquivalenciasCaracteristicasCpOtrosPuertos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual int Material_Id { get; set; }
        [Required]
        public virtual string CodigoExterno { get; set; }
        [Required]
        public virtual string CodigoSap { get; set; }
        [Required]
        public virtual bool EsHumedad { get; set; }
    }
}
