using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ActividadConCargaAutomaticaDto
    {
        public int Id { get; set; }

        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        public string WorkflowDescripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Actividad")]
        public string Actividad { get; set; }
        
    }
}
