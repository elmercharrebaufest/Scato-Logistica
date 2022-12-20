using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class HistorialMensajeCartelLedDto
    {
        public int Id { get; set; }

        public string Mensaje { get; set; }

        public DateTime? FechaUltimaModificacion { get; set; }

        public CalleDto Calle { get; set; }
    }
}
