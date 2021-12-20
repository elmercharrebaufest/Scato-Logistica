using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LlenadoMilimetroPorTanque : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string TanqueNum { get; set; }
        public virtual string Cm { get; set; }
        public virtual string Mm { get; set; }
        public virtual string LlenadoMm { get; set; }
    }
}
