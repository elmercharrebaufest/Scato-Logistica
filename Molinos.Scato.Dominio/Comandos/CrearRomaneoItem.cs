using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearRomaneoItem : Comando
    {
        public RomaneoItemDto Dto { get; set; }
    }
}
