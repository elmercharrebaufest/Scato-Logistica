using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogActividad : IIdentificable
    {
        public virtual int Id { get; set; }
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual string Actividad { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string ActividadXaml { get; set; }
    }
}
