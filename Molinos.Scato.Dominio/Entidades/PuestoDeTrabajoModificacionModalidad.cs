using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PuestoDeTrabajoModificacionModalidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual bool Automatico { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string NombreUsuarioResponsable { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
