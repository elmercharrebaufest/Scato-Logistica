using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class FiltroCapturasFallidasDto
    {
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }
    }
}
