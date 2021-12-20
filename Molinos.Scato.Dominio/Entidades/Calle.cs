using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Calle : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Nombre { get; set; }
        [Required]
        public virtual string Codigo { get; set; }
        [Required]
        public virtual int CentroId { get; set; }

        public virtual TipoCalle TipoCalle { get; set; }

        public virtual int CantidadDeCamiones { get; set; }

        public virtual bool Bloqueada { get; set; }
        public virtual bool Deshabilitada { get; set; }
        public virtual DateTime? FechaLLamada { get; set; }
        public virtual bool Automatica { get; set; }
        public virtual Material Material { get; set; } //Material para llamado automatico de fila y filas pre-calado
        public virtual TipoCalidad TipoCalidad { get; set; } //Para filas pos calado
        public virtual int HidraulicaAsignada { get; set; }

        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }

        public virtual decimal? RangoCaracteristicaCalidadMaximo { get; set; }

        public virtual decimal? RangoCaracteristicaCalidadMinimo { get; set; }
    }
}
