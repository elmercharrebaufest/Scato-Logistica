using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoComercialPorWfDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        public string WorkflowDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        [Required]
        public int TipoComercialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public string TipoComercialDescripcion { get; set; }
    }
}