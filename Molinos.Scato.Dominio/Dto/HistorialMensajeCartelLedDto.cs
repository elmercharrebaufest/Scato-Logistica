using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class HistorialMensajeCartelLedDto  
    {
        public int Id { get; set; }

        public string Mensaje { get; set; }

        public DateTime? FechaUltimaModificacion { get; set; }
        public int? RecorridoId { get; set; }
        public string CalleNombre { get; set; }
        public int CalleId { get; set; }
        public string CalleColorTexto { get; set; }
        public string CalleColorFondo { get; set; }
    }
}
