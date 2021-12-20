using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RolDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Permisos_Titulo")]
        public List<PermisoDto> PermisosAsociados { get; set; }
      
        //public IList<UsuarioDto> UsuariosAsociados { get; set; }

        [DataMember(Name = "_destroy")]
        public bool Eliminar { get; set; }
    }
}
