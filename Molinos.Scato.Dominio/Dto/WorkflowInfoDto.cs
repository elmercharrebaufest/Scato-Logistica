using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class WorkflowInfoDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string ActividadInicial { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
