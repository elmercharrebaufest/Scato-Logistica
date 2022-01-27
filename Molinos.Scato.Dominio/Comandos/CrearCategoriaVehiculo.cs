using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCategoriaVehiculo : Comando
    {
        public CategoriaVehiculoDto Dto { get; set; }
    }
}
