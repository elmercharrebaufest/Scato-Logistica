using System.Collections.Generic;
using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ListarWorkFlowsDto
    {
        public ListaPaginada<InstanciaWorkflowDto> InstanciasWorkflowDto { get; set; }
        public List<string> ProximasAcciones { get; set; }
    }
}