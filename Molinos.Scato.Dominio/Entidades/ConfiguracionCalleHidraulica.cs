using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfiguracionCalleHidraulica : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Calle Calle { get; set; }
        public virtual string CodigoCartel { get; set; }
        public virtual string CodigoSensorCamaraALPR { get; set; }
        public virtual string CodigoSensorCirculacion { get; set; }
    }
}
