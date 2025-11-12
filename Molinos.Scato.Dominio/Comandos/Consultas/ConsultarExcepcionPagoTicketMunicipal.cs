using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarExcepcionPagoTicketMunicipal : Comando
    {
        public FiltroExcepcionPagoTasaMunicipal Filtro {  get; set; }
        public Paginacion paginacion { get; set; }
    }
}

