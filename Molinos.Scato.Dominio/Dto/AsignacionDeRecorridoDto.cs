using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AsignacionDeRecorridoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AsignacionDeRecorrido_FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AsignacionDeRecorrido_FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialPorCentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calidad")]
        public string Calidad { get; set; }
        public int CalidadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow")]
        public string Workflow { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public string Calle { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CalleId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaBruto")]
        public string BalanzaBruto { get; set; }
        public int? BalanzaBrutoId { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "BalanzaTara")]
        public string BalanzaTara { get; set; }
        public int? BalanzaTaraId { get; set; }

        [Display(ResourceType = typeof (Textos), Name = "AsignacionDeRecorrido_AlmacenDestino")]
        public string AlmacenDestino { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenDestinoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoComando_Hidraulicas")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int[] HidraulicasId { get; set; }

        public int CentroId { get; set; }

    }
}
