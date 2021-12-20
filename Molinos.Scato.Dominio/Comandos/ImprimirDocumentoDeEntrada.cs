
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirDocumentoDeEntrada : Comando
    {
        public ImpDocumentoDeEntradaDto Dto { get; set; }
        public FirmaDto Firma { get; set; }
    }
}
