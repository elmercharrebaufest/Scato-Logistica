using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ActividadPorDispositivo : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual Workflow Workflow { get; set; }
        public virtual string Actividad { get; set; }
        //apertura de barreras
        public virtual string Salida { get; set; }
        //cierre de barreras
        public virtual string Entrada { get; set; }
        [InverseProperty("ActividadPorDispositivo")]
        public virtual ICollection<VideoCamara> VideoCamaras { get; set; }

        public virtual IEnumerable<string> Salidas()
        {
            return (Salida ?? string.Empty).Split(',');
        }

        public virtual IEnumerable<string> Entradas()
        {
            return (Entrada ?? string.Empty).Split(',');
        }
    }
}
