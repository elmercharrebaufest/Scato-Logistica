using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class KmPorProveedor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string KmARecorrer { get; set; }
    }
}
