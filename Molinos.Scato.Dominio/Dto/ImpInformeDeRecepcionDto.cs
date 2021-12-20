using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpInformeDeRecepcionDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroInforme { get; set; }
        public string Proveedor { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroRemito { get; set; }
        public List<ImpInformeDeRecepcionItemDto> ImpInformeDeRecepcionItems { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public string Impresora { get; set; }
        public string Codigo { get; set; }
        [NotMapped]
        public int Pagina { get; set; }
        [NotMapped]
        public int TotalPaginas { get; set; }
        [NotMapped]
        public int RomaneoId { get; set; }
    }
}
