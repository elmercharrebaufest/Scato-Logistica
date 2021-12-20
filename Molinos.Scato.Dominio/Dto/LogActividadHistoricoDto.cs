using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LogActividadHistoricoDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string Actividad { get; set; }
        public DateTime Fecha { get; set; }
        public string ActividadXaml { get; set; }
    }
}
