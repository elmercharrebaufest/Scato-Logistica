using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ArchivoDeMovimientosDto
    {

        public int Id { get; set; }

        public string NumeroDeArchivo { get; set; }

        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        public List<MovimientoDeTercerosDto> Movimientos { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public string MaterialDesc { get; set; }
    }
}
