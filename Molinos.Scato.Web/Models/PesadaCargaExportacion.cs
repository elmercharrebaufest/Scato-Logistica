using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class PesadaCargaExportacion
    {
        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { set; get; }
        [Display(ResourceType = typeof(Textos), Name = "Peso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? Peso { set; get; }
        public Guid WorkflowInstanceId { set; get; }
        public string ActividadXaml { set; get; }
        public TipoVehiculo TipoVehiculo { set; get; }
        public bool Rechazado { set; get; }
        public TipoDocumentoIngreso DocumentoIngreso { get; internal set; }
        public string NumeroDocumentoIngreso { get; internal set; }
        public string Material { get; internal set; }
        public string TipoComercial { get; internal set; }
        public string Workflow { get; internal set; }
        public int WorkflowDefinicionId { get; internal set; }
        public string TieneEntregador { get; internal set; }
        public string PatenteOriginal { get; internal set; }
        public int BalanzaId { get; internal set; }
        public int Modalidad { set; get; }
        public string Balanza { get; internal set; }
        public bool EstaEnCero { get; internal set; }
        public string Color { get; internal set; }
    }
}