using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfiguracionDeTabla : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Material { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<CaracteristicaDeCalidad> CaracteristicasDeCalidad  { get; set; }
    }
}
