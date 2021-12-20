using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaracteristicasCartaPorteOtrosPuertos : IIdentificable
    {
        [Key]
        public virtual int Id {get;set;}
        [Required]
        public virtual int CartaPorteOtrosPuertos_Id { get; set; }
        [ForeignKey ("CartaPorteOtrosPuertos_Id")]
        public virtual CartaPorteOtrosPuertos CartaPorteOtrosPuertos { get; set; }
        [Required]
        public virtual string CodigoExterno { get; set; }
        [Required]
        public virtual decimal Valor { get; set; }

    }
}
