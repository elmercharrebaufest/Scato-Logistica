using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class Pesada
    {
        [Display(ResourceType = typeof(Textos), Name = "Balanza_Titulo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int BalanzaId { set; get; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ProximaBalanzaId { set; get; }
        public string ProximaBalanzaDesc { set; get; }
        public Modalidad Modalidad { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { set; get; }
        public string PatenteOriginal { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Peso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? Peso { set; get; }
        public TipoPesada TipoPesada { set; get; }
        public Guid WorkflowInstanceId { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Pesada_Almacen")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenId { set; get; }
        public string AlmacenDesc { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Hidraulica")]
        public int? HidraulicaId { set; get; }
        public string HidraulicaDesc { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public int? CalleId { set; get; }
        public string CalleDesc { set; get; }
        public bool ControlPesada { set; get; }
        public string BalanzaPuestoDeTrabajo { set; get; }
        public string ActividadXaml { set; get; }
        public TipoVehiculo TipoVehiculo { set; get; }
        public bool Rechazado { set; get; }

        public string Balanza { get; internal set; }
        public string Color { get; internal set; }

        public string Mensaje { get; set; }
        public string Comentario { get; set; }
        public TipoDeWorkflow TipoDeWorkflow { get; internal set; }

        [Display(ResourceType = typeof(Textos), Name = "Motivo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MotivoPesadaManual { get; set; }
    }
}