using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class RechazarRomaneoItem : Comando
    {
        public IList<RomaneoItemDto> Dto { get; set; }
    }
}
