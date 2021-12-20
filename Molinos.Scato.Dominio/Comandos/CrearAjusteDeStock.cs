using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearAjusteDeStock : Comando
    {
        public AjusteDeStockDto Dto { get; set; }
    }
}
