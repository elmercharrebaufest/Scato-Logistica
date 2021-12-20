using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class WorkflowDefinicionDto
    {
        public int Id { get; set; }
        public WorkflowDto Workflow { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Comentario { get; set; }
        public string NombreUsuario { get; set; }
        public string ActividadInicial { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaActivacion { get; set; }
    }
}
