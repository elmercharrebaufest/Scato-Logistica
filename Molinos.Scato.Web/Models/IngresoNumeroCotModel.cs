using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class IngresoNumeroCotModel
    {
        public Guid InstanciaWorkflow { set; get; }
        public int WorkflowDefinicionId { set; get; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NumeroCot { set; get; }
        public string Mensaje { set; get; }
    }
}