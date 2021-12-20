using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpControlDeCargaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public Guid WorkflowId { get; set; }
        public string NumeroControl { get; set; }
        public string Centro { get; set; }
        public string Patente { get; set; }
        public DateTime FechaDocumentoDeIngreso { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? PesoTara { get; set; }
        public decimal? PesoNeto { get; set; }
        public decimal? TotalDescargado { get; set; }
        public decimal? Diferencia { get; set; }
    }
}
