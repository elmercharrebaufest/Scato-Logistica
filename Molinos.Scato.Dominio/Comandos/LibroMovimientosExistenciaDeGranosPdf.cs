using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class LibroMovimientosExistenciaGranosPdf : ImprimirLibroMovimientosExistenciaGranos
    {
        public List<ImpImpresionGenericaDto> Dtos { get; set; }
        public string Impresora { get; set; }
        public bool EsPdf { get; set; }
        public FormatoDeImpresionDto FormatoDeImpresion { get; set; }
    }
}
