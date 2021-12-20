using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Rol : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }

        [InverseProperty("RolesAsociados")]
        public virtual IList<Permiso> PermisosAsociados { get; set; }
        public virtual IList<Usuario> UsuariosAsociados { get; set; }
    }
}
