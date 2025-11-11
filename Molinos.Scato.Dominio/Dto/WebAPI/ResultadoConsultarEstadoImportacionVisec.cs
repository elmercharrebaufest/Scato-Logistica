using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoConsultarEstadoImportacionVisec
    {
        public List<EstadoImportacionDetalle> Detalles { get; set; }
        public int NumeroProceso { get; set; }
        public DateTime FechaHora { get; set; }
        public int CodigoEstado { get; set; }
        public string DescripcionEstado { get; set; }
    }

    public class EstadoImportacionDetalle
    {
        public List<DetalleError> Errores { get; set; }
    }

    public class DetalleError
    {
        public string Observaciones { get; set; }
    }
}