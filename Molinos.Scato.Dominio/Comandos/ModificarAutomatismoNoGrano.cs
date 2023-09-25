using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAutomatismoNoGrano : Comando
    {
        public AutomatismoNoGranoDto Dto { get; set; }
    }
}
