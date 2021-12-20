using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class FechaModel
    {
        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }
    }
}