using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    public class LeerNumeroCartaPorte : Comando
    {
        public string NombreArchivo { get; set; }
        public byte[] CodigoBarrasCartaPorte { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool CalcularRecorte { get; set; }
    }
}
