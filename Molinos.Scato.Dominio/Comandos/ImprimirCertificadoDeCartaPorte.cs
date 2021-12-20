
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirCertificadoDeCartaPorte : Comando
    {
        public ImpCertificadoDeCartaPorteDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
