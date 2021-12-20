using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class EstadoPuertoDto
    {
        public int Id { get; set; }
        public DateTime? FechaCalado { get; set; }
        public DateTime? FechaUbicacion { get; set; }
        public DateTime? FechaAlturaRio { get; set; }
        
        public string Calado { get; set; }
        public string Ubicacion { get; set; }
        public string AlturaDelRio { get; set; }

    }
}