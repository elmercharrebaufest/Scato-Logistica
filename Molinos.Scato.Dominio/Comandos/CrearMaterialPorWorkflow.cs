using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearMaterialPorWorkflow : Comando
    {
        public MaterialPorWorkflowDto Dto { get; set; }
    }
}
