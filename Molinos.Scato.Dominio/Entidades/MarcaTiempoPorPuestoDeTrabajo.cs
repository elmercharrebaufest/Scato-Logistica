using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MarcaTiempoPorPuestoDeTrabajo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual int? PuestoDeTrabajoId { get; set; }

        public virtual DateTime? FechaInicio { get; set; }

        public virtual DateTime? FechaIdentificacion { get; set; }

        public virtual DateTime? FechaFin { get; set; }

        public virtual TipoIdentificacionPorPuesto? TipoIdentificacion { get; set; }

        public virtual int? RecorridoId { get; set; }
    }
}
