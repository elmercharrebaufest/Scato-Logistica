using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class MonitorCPECacheadaListadoDto
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public long? CTG { get; set; }
        public int? CodigoONCA { get; set; }
        public string MaterialDescripcion { get; set; }
        public bool TienePDF { get; set; }
        public DateTime? FechaCacheado { get; set; }
        public DateTime? FechaCPE { get; set; }
    }
}