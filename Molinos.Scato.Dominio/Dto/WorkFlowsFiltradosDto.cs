using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class WorkFlowsFiltradosDto
    {
        public IEnumerable<InstanciaWorkflowDto> InstanciasWorkflowDto { get; set; }
    }
}