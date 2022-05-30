using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarImagenDescarga : Comando
    {
        public string NroCartaPorte { get; set; }
        public string RutaFotoCP { get; set; }
        public string CodigoCentroSap { get; set; }
        public string Patente { get; set; }
        public bool EsSustentable { get; set; }
        public byte[] Pdf { get; set; }
    }
}
