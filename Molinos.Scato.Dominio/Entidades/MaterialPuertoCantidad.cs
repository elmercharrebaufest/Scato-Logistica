using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MaterialPuertoCantidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual Embarque Embarque { get; set; }
        public virtual string Color { get; set; }
    }
}
