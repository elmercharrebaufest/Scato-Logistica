using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LogIngresoPorPuestoDto
    {
        public int Id { get; set; }
        public int RecorridoId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public TipoIngresoPorPuesto TipoIngreso { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
