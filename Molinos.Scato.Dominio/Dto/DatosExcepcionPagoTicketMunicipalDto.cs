using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Entidades;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosExcepcionPagoTicketMunicipalDto
    {
        public ListaPaginada<LogExceptuadosTicketMunicipalDto> Exceptuados { get; set; }
        public IList<CalidadMaterialDto> Calidades { get; set; }
        public IList<WorkflowDto> WorkflowsCentro { get; set; } 
    }
}