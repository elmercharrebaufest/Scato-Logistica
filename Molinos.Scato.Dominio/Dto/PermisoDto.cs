using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PermisoDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Permiso_TipoPermiso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoPermiso TipoPermiso { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public PermisosScato? Codigo { get; set; }  //TODO: FIJARSE EL NOMBRE NO CONCUERDA CON EL PERMISOSCATO POR ESO TRAE NULLO

        [DataMember]
        [Display(ResourceType = typeof(Textos), Name = "Actividad")]
        public string ActividadWorkflow { get; set; }

    }
}
