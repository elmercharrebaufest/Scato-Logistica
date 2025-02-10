using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarOrdenesNoGranosCargaDeCupo : Comando
    {
        public string Patente { get; set; }
        public int CentroId { get; set; }
    }

    public class ResultadoConsultarOrdenesNoGranosCargaDeCupo : Resultado
    {
        public List<OrdenNoGranosCargaDeCupoDto> Ordenes { get; set; } = new List<OrdenNoGranosCargaDeCupoDto>();
    }
}
