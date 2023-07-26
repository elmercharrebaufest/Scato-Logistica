using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class AuditoriaBaseDto
    {
        public bool Borrado { get; set; }
        public string CreadoPor { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}