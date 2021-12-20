using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearTipoComercialPorWf : Comando
    {
        public TipoComercialPorWfDto Dto { get; set; }
    }
}
