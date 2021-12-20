using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarMaterial : Comando
    {
        public MaterialDto Dto { get; set; }
        public MaterialPorCentroDto MaterialPorCentroDto { get; set; }
    }
}
