using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DistribucionDeAlmacen : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual DistribucionDeAlmacenes DistribucionDeAlmacenes { get; set; }
        public virtual Almacen Almacen { get; set; }
        public virtual int Litros { get; set; }
    }
}
