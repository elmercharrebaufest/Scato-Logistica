using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ImpresionDto
    {
        public int Id { get; set; }
        public DateTime FechaImpresion { get; set; }
        public TipoImpresion TipoImpresion { get; set; }
        public string Patente { get; set; }
        public bool Eliminada { get; set; }
    }
}