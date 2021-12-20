using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarMaterialPorWorkflow : Comando
    {
        public MaterialPorWorkflowDto Dto { get; set; }
    }
}
