using System;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AuditoriaBase
    {
        public virtual bool Borrado { get; set; }
        public virtual string CreadoPor { get; set; }
        public virtual DateTime? FechaCreacion { get; set; }
        public virtual string ModificadoPor { get; set; }
        public virtual DateTime? FechaModificacion { get; set; }
    }
}