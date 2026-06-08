using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MarcaTiempoPorPuestoDeTrabajo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string NumeroDocumento { get; set; }

        public virtual int PuestoDeTrabajoId { get; set; }

        public virtual DateTime? FechaInicio { get; set; }

        public virtual DateTime? FechaFin { get; set; }

        public virtual int? Centro_Id { get; set; }

        public virtual TipoIngresoPorPuesto TipoIngreso { get; set; }
    }
}
