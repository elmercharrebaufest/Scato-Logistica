using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarCPEPorDestino : Comando
    {
        public TipoCpeConsulta TipoCpe { get; set; }
        public int CentroId { get; set; }
        public DateTime FechaPartidaDesde { get; set; }
        public DateTime FechaPartidaHasta { get; set; }
    }

    public enum TipoCpeConsulta
    {
        Camion,
        Tren,
        Ambos
    }
}