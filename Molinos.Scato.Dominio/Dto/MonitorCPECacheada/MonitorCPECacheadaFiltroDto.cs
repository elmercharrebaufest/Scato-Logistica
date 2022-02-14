using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class MonitorCPECacheadaFiltroDto
    {
        public DateTime? FechaCPEDesde { get; set; }
        public DateTime? FechaCPEHasta { get; set; }
        public int? MaterialId { get; set; }
        public long? CTG { get; set; }
        public string Patente { get; set; }
        public bool VerCamionesPorLlegar { get; set; }
        public int? CentroId { get; set; }

        //CAMPOS PARA DEVOLVER
        public int CamionesPendientes { get; set; }
        public DateTime? FechaEjecucionCacheoCPE { get; set; }
        public string ErrorCacheoAfipCPE { get; set; }
    }
}