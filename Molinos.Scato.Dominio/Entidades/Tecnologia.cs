using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Tecnologia : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Codigo { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
