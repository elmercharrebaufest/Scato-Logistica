using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarAjusteDeStock : Comando
    {
        public AjusteDeStockDto Dto { get; set; }
    }
}
