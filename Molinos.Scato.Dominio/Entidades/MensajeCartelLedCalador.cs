using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MensajeCartelLedCalador : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [Column("MensajeCartelLed_Id")]
        public virtual int MensajeCartelLedId { get; set; }
        public virtual MensajeCartelLed MensajeCartelLed { get; set; }

        [Column("Calle_Id")]
        public virtual int CalleId { get; set; }
        public virtual Calle Calle { get; set; }
    }
}
