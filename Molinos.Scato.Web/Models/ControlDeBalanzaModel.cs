using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class ControlDeBalanzaModel
    {
        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public Guid InstanciaWorkflow { set; get; }
        public int WorkflowDefinicionId { set; get; }
        public bool ControlBalanza { set; get; }
    }
}