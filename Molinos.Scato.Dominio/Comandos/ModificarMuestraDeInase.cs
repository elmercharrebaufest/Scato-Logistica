using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMuestraDeInase : Comando
    {
        public IList<int> Lista { get; set; }
    }
}
