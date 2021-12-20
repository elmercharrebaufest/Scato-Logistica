using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirDocumentoDeImpresion : Comando
    {
        public ImpImpresionGenericaDto Dto { get; set; }
        public FormatoDeImpresionDto FormatoDeImpresion { get; set; }
        public int CantidadCopias { get; set; }
    }
}
