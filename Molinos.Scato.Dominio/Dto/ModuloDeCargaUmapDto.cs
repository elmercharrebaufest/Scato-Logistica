using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaUmapDto
    {
        public int Id { get; set; }
        public DateTime? FechaEncendido { get; set; }
        public string HoraEncendido { get; set; }
        public DateTime? FechaApagado { get; set; }
        public string HoraApagado { get; set; }
    }
}