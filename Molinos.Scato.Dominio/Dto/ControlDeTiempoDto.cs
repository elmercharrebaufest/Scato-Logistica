using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class ControlDeTiempoDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        public string WorkflowDescripcion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "ControlDeTiempo_ActividadDesde")]
        public string ActividadDesde { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "ControlDeTiempo_ActividadHasta")]
        public string ActividadHasta { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "ControlDeTiempo_TiempoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int TiempoMaximo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "ControlDeTiempo_ActividadDeControl")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoControl { get; set; }
    }
}
