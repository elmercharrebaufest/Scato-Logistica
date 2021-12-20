using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarHumedimetro : Comando
    {
        public HumedimetroDto Dto { get; set; }
    }
}
