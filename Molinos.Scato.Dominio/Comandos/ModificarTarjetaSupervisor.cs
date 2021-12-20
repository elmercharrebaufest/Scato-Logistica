using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarTarjetaSupervisor : Comando
    {
        public TarjetaSupervisorDto Dto { get; set; }
    }
}
