using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirDocumentoDeImpresionModelo : Comando
    {
        public FormatoDeImpresionDto Formato { get; set; }
        public string Impresora { get; set; }
    }
}
