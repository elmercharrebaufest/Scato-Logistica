using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearActualizarVisecTransmision : Comando
    {
        public VisecTransmisionDto Dto { get; set; }
    }
}