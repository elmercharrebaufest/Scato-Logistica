using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ColaImpresionDto
    {
        public string Documento { get; set; }
        public string Estado { get; set; }
        public int Paginas { get; set; }
        public int Tamanio { get; set; }
        public DateTime Fecha { get; set; }
        public int JobId { get; set; }
        public string Servidor { get; set; }
        public string ServidorUrl { get; set; }
        public int ImpresoraId { get; set; }
    }
}
