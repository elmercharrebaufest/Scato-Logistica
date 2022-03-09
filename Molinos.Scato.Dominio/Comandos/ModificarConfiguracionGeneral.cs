using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarConfiguracionGeneral : Comando
    {
        public ConfiguracionGeneralDto Dto { get; set; }
    }
}
