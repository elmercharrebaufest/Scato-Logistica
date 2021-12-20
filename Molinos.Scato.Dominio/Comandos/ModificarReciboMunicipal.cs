using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class ModificarReciboMunicipal : Comando
    {
        public ReciboMunicipalDto Dto { get; set; }
    }
}
