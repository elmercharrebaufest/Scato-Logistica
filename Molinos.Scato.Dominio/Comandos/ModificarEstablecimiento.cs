using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarEstablecimiento : Comando
    {
        public EstablecimientoDto Dto { get; set; }
    }
}
