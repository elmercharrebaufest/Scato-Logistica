using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Casillero : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Numero { get; set; }
        public virtual int Capacidad { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
