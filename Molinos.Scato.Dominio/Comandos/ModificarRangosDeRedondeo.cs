using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarRangosDeRedondeo : Comando
    {
        public RangosDeRedondeoDto Dto { get; set; }
    }
}
