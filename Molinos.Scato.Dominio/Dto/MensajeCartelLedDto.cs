using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class MensajeCartelLedDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int Orden { get; set; }
        public string Mensaje { get; set; }
        public string Programa { get; set; }
        public string Trama { get; set; }
        public string Variable { get; set; }
        public int SegundosDeEspera { get; set; }
        public string DescripcionFormatoMensaje { get; set; }
        public bool Habilitado { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public HistorialMensajeCartelLedDto HistorialMensajeCartelLed { get; set; }
    }
}
