using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearReciboMunicipal : Comando
    {
        public ReciboMunicipalDto Dto { get; set; }
    }
}
