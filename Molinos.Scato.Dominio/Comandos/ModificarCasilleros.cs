using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCasilleros : Comando
    {
        public IList<CasilleroDto> Dto { get; set; }
        public int Capacidad { get; set; }
    }
}
