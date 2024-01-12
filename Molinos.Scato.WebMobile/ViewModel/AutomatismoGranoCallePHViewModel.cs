using Molinos.Scato.Dominio.Recursos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class AutomatismoGranoCallePHViewModel
    {
        public AutomatismoGranoCallePHViewModel()
        {
            ListaAutomatismoTipoLlamado = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "CallePreHidraulica")]
        public string Descripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(1, int.MaxValue)]
        public int Camiones { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoLlamado")]
        public int? AutomatismoTipoLlamadoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoLlamado")]
        public List<SelectListItem> ListaAutomatismoTipoLlamado { get; set; }
    }
}