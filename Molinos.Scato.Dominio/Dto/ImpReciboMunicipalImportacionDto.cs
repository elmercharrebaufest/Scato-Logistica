using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpReciboMunicipalImportacionDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Ordenanza { get; set; }
        public string Valor { get; set; }
        public string TicketNro { get; set; }
        public string NroDocumentoLegal { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string Patente { get; set; }
    }
}
