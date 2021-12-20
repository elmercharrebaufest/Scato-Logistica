using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CaracteristicaDeCalidadPorWorkflowDto
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Workflow")]
        public string WorkflowDesc { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }
        public string WorkflowTipo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad")]
        public string CaracteristicaDeCalidadDesc { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CaracteristicaDeCalidadId { get; set; }

        public List<int> CaracteristicasDeCalidadId { get; set; }
    }
}
