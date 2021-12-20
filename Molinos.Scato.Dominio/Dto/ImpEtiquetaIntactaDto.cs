
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpEtiquetaIntactaDto
    {
        public int Id { get; set; }
        public string Centro { get; set; }
        public string Material { get; set; }
        public string NumeroCartaPorte { get; set; }
        public string TipoDeAnalisis { get; set; }
        public string LaboratiorioCuit { get; set; }
        public string LaboratorioNombre { get; set; }
        public string Impresora { get; set; }
        public DateTime FechaImpresion { get; set; }
        public Guid WorkflowId { get; set; }
        public string Codigo { get; set; }
        public string Patente { get; set; }
        public string NombreUsuario { get; set; }
    }
}
