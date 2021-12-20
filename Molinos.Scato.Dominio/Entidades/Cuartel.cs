using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Cuartel : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual VinedoPropio VinedoPropio { get; set; }
        public virtual bool Activo { get; set; }
    }
}
