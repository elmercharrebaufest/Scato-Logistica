using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FiltroLoteInaseDto
    {
        public int LoteId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string NroLote { get; set; }
        public int CentroId { get; set; }
    }
}
