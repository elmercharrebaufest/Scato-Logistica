using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class HistorialMensajeCartelLed : IIdentificable
    {
        [Key, ForeignKey("MensajeCartelLed")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual string Mensaje { get; set; }

        public virtual DateTime? FechaUltimaModificacion { get; set; }

        public virtual Calle Calle { get; set; }

        [Required]
        public virtual MensajeCartelLed MensajeCartelLed { get; set; }
    }
}
