using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCasilleros : Comando
    {
        public IList<CasilleroDto> Dto { get; set; }
    }
}
