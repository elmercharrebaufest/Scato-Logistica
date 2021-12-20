using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Variedad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string NumeroINV { get; set; }
    }
}
