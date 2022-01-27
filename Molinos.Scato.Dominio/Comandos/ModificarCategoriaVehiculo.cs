using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarCategoriaVehiculo : Comando
    {
        public CategoriaVehiculoDto Dto { get; set; }
    }
}
