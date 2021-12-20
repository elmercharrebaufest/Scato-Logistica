using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ControlRecorrido : IIdentificable
    {
        public virtual int Id { get; set; }
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual string Actividad { get; set; }
        public virtual string ActividadXaml { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual string Comentario { get; set; }
        public virtual bool Decision { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
    }
}
