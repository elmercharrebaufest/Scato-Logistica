using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class VisecTransmisionDto : VisecTransmisionBaseDto
    {
        public DateTime? FechaTransaccion { get; set; }
        public string NumeroProceso { get; set; }
        public string DetalleTransaccion { get; set; }
        public string HistorialProcesos { get; set; }
        public string DocumentosAsociados { get; set; }
        public int? StockKg { get; set; }
    }
}