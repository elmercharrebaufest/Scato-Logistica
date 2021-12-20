using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpTicketPesadaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string NumeroIngreso { get; set; }
        public string Emisor { get; set; }
        public string DoimicilioCentro { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Material { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTara { get; set; }
        public string PesoNeto { get; set; }
        public string Remitente { get; set; }
        public string Transportista { get; set; }
        public string CuitTransportista { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Observaciones { get; set; }
        public IdentidadDeCopia Identidad { get; set; }//original-duplicado-etc
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public virtual string Codigo { get; set; }
    }
}
