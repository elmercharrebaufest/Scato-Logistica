
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirCartaPorteUrenport : Comando
    {
        public ImpCartaPorteUrenportDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
