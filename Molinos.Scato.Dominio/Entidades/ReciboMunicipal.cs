using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ReciboMunicipal : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual decimal Monto { get; set; }
        [Required]
        public virtual string Ordenanza { get; set; }
        public virtual Centro Centro { get; set; }
        [Required]
        public virtual DateTime FechaActivacion { get; set; }
        public virtual TipoVehiculo? TipoVehiculo { get; set; }
    }
}
