using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CubitacionDeTanques : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Tk { get; set; }
        public virtual double Altura { get; set; }
        public virtual double Cantidad { get; set; }
    }
}