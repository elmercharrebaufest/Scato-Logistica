using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpCartaPorteUrenportDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string Patente { get; set; }
        public string FotoRutaDestino { get; set; }
    }
}
