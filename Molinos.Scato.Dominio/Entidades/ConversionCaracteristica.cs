using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConversionCaracteristica : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual CaracteristicaDeCalidad Caracteristica { get; set; }
        public virtual Material Material { get; set; }
        public virtual string CodigoCamara { get; set; }
    }
}
