using System.Collections.Generic;
using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosDeWorkflowsDto
    {
        public ListaPaginada<InstanciaWorkflowDto> Workflows { get; set; }
        public IList<CalidadMaterialDto> Calidades { get; set; }
        public IList<WorkflowDto> WorkflowsCentro { get; set; } 
    }
}