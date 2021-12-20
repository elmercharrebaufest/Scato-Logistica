
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirFormulario239 : Comando
    {
        public ImpFormulario239Dto Dto { get; set; }
        public int CantidadCopias { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
