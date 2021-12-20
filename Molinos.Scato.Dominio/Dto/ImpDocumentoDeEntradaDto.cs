using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpDocumentoDeEntradaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroDeIngreso { get; set; }
        public string Patente { get; set; }
        public DateTime FechaDocumentoDeIngreso { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
    }
}
