using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarVariedadPorMaterial : Comando
    {
        public int IdMaterial { get; set; }
        public TipoVariedadDto[] Dto { get; set; }
    }
}
