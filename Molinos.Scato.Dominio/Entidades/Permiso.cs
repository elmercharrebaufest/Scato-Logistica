using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Permiso : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }

        [Required]
        public virtual TipoPermiso TipoPermiso { get; set; }
        
        [Required]
        public virtual PermisosScato Codigo { get; set; }

        public virtual string ActividadWorkflow { get; set; }

        public virtual IList<Rol> RolesAsociados { get; set; }
    }
}
