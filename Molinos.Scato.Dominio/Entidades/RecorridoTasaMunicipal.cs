using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RecorridoTasaMunicipal : IIdentificable
    {
        [Key]
        [ForeignKey("Recorrido")]
        public virtual int Id { get; set; }

        public virtual bool Exceptuado { get; set; }
        public virtual string MotivoExceptuado { get; set; }

        public virtual Recorrido Recorrido { get; set; }
    }
}