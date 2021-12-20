using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConversionGrupo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual Material Material { get; set; }
        public string CodigoSegunCamara { get; set; }
    }
}
