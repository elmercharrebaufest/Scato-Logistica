using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EstadoPuerto : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime? FechaCalado { get; set; }
        public virtual string Calado { get; set; }
        public virtual DateTime? FechaUbicacion { get; set; }
        public virtual string Ubicacion { get; set; }
        public virtual DateTime? FechaAlturaRio { get; set; }
        public virtual string AlturaDelRio { get; set; }
    }
}
