using Molinos.Scato.Dominio.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class SensorMarcaTiempoPorPuestoDeTrabajo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string CodigoSensor { get; set; }

        [ForeignKey("PuestoDeTrabajo")]
        public virtual int PuestoDeTrabajoId { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public virtual TipoSensorMarcaTiempo TipoSensor { get; set; }
    }
}
