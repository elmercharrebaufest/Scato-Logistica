using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirFileGenerico : Comando
    {
        public FormatoDeImpresionDto FormatoDeImpresion { get; set; }
        public int CantidadCopias { get; set; }
        public string Impresora { get; set; }
        public string CodigoDocumentoImpresion { get; set; }
        public byte[] File { get; set; }
    }
}
