using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosDeWorkflowDto
    {
        public Guid InstanciaWorkflow { get; set; }
        public int WorkflowDefId { get; set; }
        public string Codigo { get; set; }
        public string Patente { get; set; }
        public DateTime FechaCalado { get; set; }
        public string PatenteAcoplado { get; set; }
        public string MaterialDescripcion { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Calidad { get; set; }
        public string Humedad { get; set; }
        public bool MaterialEsGrano { get; set; }
        public bool TieneDescuentos { get; set; }
        public bool EsSoja { get; set; }
    }
}