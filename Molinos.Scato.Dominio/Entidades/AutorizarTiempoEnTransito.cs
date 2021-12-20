using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AutorizarTiempoEnTransito : IIdentificable
    {
        public virtual int Id { get; set; }
        public virtual Guid WorkflowInstanceId { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual string Actividad { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual string Comentario { get; set; }
        public virtual bool Decision { get; set; }
        public virtual DateTime Fecha { get; set; }

        public virtual string DocumentoIngreso { get; set; }
        public virtual string NroDocumentoIngreso { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Material { get; set; }
        public virtual string Actividad1 { get; set; }
        public virtual string Actividad2 { get; set; }
        public virtual TimeSpan TiempoAceptado { get; set; }
        public virtual TimeSpan TiempoEnTransito { get; set; }
        public virtual string FechaActividad1 { get; set; }
        public virtual string FechaActividad2 { get; set; }
        public virtual TimeSpan DiferenciaTiempo { get; set; }
    }
}
