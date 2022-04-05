using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AgregarMarcaSustentable : Comando
    {
        public string RutaFotoCP { get; set; }
        public string CodigoCentroSap { get; set; }
        public string NroDocumento { get; set; }
        public string Patente { get; set; }
        public bool SoloDibujar { get; set; }
        public byte[] PdfImage { get; set; }
    }
}
