using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearCaracteristicaDeCalidadPorWorkflow : Comando
    {
        public CaracteristicaDeCalidadPorWorkflowDto Dto { get; set; }
    }
}
