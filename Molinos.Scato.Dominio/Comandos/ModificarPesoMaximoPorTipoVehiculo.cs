using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarPesoMaximoPorTipoVehiculo : Comando
    {
        public PesoMaximoPorTipoVehiculoDto Dto { get; set; }
    }
}
