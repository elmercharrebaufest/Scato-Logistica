using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearEstablecimiento : Comando
    {
        public EstablecimientoDto Dto { get; set; }
    }
}
