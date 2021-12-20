using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DocumentoExternoDto
    {
        public int Id { get; set; }
        public string NumeroDeDocumento { get; set; }
        public string ArchivoRutaDestino { get; set; }
        public string ArchivoExtension { get; set; }
        public string NombreArchivoOriginal { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public DateTime Fecha { get; set; }

        public byte[] Archivo { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool CalcularRecorte { get; set; }
    }
}