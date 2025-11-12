using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearExcepcionPagoTasaMunicipal : Comando
    {
        public ExceptuadosTicketMunicipalDto ExceptuadosTicketMunicipalDto { get; set; }
    }
}
