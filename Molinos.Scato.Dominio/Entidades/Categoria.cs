using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Categoria : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Clasificacion { get; set; }
    }
}