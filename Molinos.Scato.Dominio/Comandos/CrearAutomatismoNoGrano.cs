using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearAutomatismoNoGrano : Comando
    {
        public AutomatismoNoGranoDto Dto { get; set; }
    }
}
