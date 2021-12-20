using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class NirsModificacionModalidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Nirs Nirs { get; set; }
        public virtual Modalidad Modalidad { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string NombreUsuarioResponsable { get; set; }
    }
}
