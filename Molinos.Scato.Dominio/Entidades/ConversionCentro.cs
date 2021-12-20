using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConversionCentro: IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string CodigoCamara { get; set; }
    }
}
