using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCoordinadorPuerto : Comando
    {
        public CoordinadorPuertoDto Dto { get; set; }
    }
}