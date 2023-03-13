using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarConfiguracionSensores : Comando
    {
        public ConfigSensoresDto Dto { get; set; }
    }
}
