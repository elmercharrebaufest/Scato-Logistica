using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpResumenHojaDeRutaDto
    {
        public string Impresora { get; set; }
        public string Codigo { get; set; }
        public int CentroId { get; set; }
        public List<DatosDeWorkflowDto> DatosDeWorkflows { get; set; }

        public int PuestoDeTrabajoId { get; set; }
    }
}
