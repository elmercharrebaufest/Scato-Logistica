using System;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarTicketsNoGranos : Comando
    {
        public string Patente { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
