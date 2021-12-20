using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarTransmisionASap : Comando
    {
        public TransmisionASapDto Dto { get; set; }
    }
}
