using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogIdentificacionVehicular : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string CodigoDispositivo { get; set; }

        public virtual string Tarjeta { get; set; }

        public virtual string ErrorDispositivo { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        public virtual string Patente { get; set; }

        public virtual bool VehiculoPresente { get; set; }

        public virtual DateTime FechaEvento { get; set; }

        public virtual string ResultadoWorkflow { get; set; }

        public virtual string PatenteLeida { get; set; }

        public virtual int DiferenciaSustitucion { get; set; }

        public virtual int? DuracionMecanismoSustitucionMs { get; set; }

        public virtual ICollection<LogIdentificacionVehicularDetalle> Detalles { get; set; }
    }
}