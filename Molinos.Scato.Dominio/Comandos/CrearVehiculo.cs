using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearVehiculo : Comando
    {
        public VehiculoDto Dto { get; set; }
    }
}
