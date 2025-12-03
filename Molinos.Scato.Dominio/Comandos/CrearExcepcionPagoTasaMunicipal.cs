using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearExcepcionPagoTasaMunicipal : Comando
    {
        public ExceptuadosTicketMunicipalDto ExceptuadosTicketMunicipalDto { get; set; }
    }
}
