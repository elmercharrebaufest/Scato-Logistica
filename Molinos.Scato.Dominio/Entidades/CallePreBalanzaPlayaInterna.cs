using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CallePreBalanzaPlayaInterna : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public virtual Calle CallePlayaInterna { get; set; }
        public virtual Calle CallePreBalanza { get; set; }

        [Column("CallePlayaInterna_Id")]
        public virtual int CallePlayaInternaId { get; set; }

        [Column("CallePreBalanza_Id")]
        public virtual int CallePreBalanzaId { get; set; }

        public virtual DateTime FechaLlamado { get; set; }
    }
}
