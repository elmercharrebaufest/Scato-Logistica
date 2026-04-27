using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarCPDigital : Comando
    {
        public int CentroId { get; set; }
        public long? NroCtg { get; set; }
        public int TipoVehiculo { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public bool ConsultaFerroviarioPorCtg { get; set; }
        public string Patente { get; set; }
        public int? MaterialId { get; set; }
    }
}