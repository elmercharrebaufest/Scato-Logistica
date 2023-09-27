using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AsignacionAutomatismoGranoEnRecorrido : IIdentificable
    {
        [Key]
        public int Id { get; set; }

        [Column("Recorrido_Id")]
        public int RecorridoId { get; set; }

        [Column("CallePreBalanza_Id")]
        public int CallePreBalanzaId { get; set; }

        [Column("CallePreHidraulica_Id")]
        public int CallePreHidraulicaId { get; set; }

        public Recorrido Recorrido { get; set; }
        public Calle CallePreBalanza { get; set; }
        public Calle CallePreHidraulica { get; set; }
    }
}