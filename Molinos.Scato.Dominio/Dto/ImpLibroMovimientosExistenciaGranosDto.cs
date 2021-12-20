using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ImpLibroMovimientosExistenciaGranosDto
    {
        public int Id { get; set; }
        public List<ImpImpresionGenericaDto> Dtos { get; set; }
        public DateTime FechaImpresion { get; set; }
        public TipoImpresion TipoImpresion { get; set; }
        public string Patente { get; set; }
        public string Impresora { get; set; }
        public bool Eliminada { get; set; }
        public bool EsPdf { get; set; }
    }
}