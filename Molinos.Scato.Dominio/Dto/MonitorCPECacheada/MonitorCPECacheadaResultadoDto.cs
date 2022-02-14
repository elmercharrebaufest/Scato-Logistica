using Molinos.Scato.Dominio.Consultas;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class MonitorCPECacheadaResultadoDto
    {
        public int CamionesPendientes { get; set; }
        public DateTime? FechaUltimaEjecucion { get; set; }
        public string ErrorCacheoAfipCPE { get; set; }
        public ListaPaginada<MonitorCPECacheadaListadoDto> MonitorCPECacheadaListado { get; set; }
    }
}