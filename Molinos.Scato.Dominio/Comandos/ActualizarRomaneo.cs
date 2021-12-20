using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarRomaneo : Comando
    {
        public RomaneoDto Dto { get; set; }
    }
}
