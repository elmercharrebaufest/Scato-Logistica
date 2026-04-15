using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LogValidacionAccesoStopBandasHorariasDto
    {
        public string CTG { get; set; }
        public string Patente { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool? Permitido { get; set; }
        public string Semaforo { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
        public DateTime? BandaHorariaFecha { get; set; }
        public string BandaHorariaHoraDesde { get; set; }
        public string BandaHorariaHoraHasta { get; set; }
        public string RespuestaStop { get; set; }
    }
}
