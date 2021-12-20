using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpResumenDeRecepcionDto
    {
        public DateTime FechaRomaneo { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroRomaneo { get; set; }
        public string CantidadComprobada { get; set; }
        public string Bultos { get; set; }
        public string Proveedor { get; set; }
        public string Estado { get; set; }
        public string Impresora { get; set; }
        public string Codigo { get; set; }
        public List<ImpResumenDeRecepcionItemDto> ImpResumenDeRecepcionItems { get; set; }

    }
}
