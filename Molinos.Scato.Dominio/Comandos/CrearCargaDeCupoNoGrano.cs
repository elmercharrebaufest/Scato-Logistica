using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearCargaDeCupoNoGrano : Comando
    {
        public CargaDeCupoDto Dto { get; set; }
    }
}
