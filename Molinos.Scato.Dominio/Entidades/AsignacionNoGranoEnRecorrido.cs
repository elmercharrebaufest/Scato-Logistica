using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AsignacionNoGranoEnRecorrido : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Calle CallePlanta { get; set; }

        [Column("CallePlanta_Id")]
        public virtual int? CallePlantaId { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        [Column("Recorrido_Id")]
        public virtual int RecorridoId { get; set; }

        public bool AplicaConteo { get; set; }
    }
}