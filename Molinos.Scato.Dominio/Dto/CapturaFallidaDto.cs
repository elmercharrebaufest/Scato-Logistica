using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class CapturaFallidaDto
    {
        public int DetalleId { get; set; }
        public string RutaImagen { get; set; }
        public string CodigoCamara { get; set; }
        public DateTime FechaEvento { get; set; }
        public int TotalRegistros { get; set; }
    }
}
