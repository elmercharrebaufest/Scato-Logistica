using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarStockDeEstablecimiento : Comando
    {
        public StockDeEstablecimientoDto Dto { get; set; }
    }
}
