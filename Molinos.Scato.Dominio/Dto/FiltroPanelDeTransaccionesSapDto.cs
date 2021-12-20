using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class FiltroPanelDeTransaccionesSapDto : IValidatableObject
    {
        public int? CentroId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_Transmision")]
        public EstadoTransmisionASap? EstadoTransmisionASap { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_Transmision")]
        public EstadoTransmisionCTG? EstadoTransmisionCTG { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_TipoDoc")]
        public TipoDocumentoIngreso? TipoDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_NroDoc")]
        public string NumeroDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_FechaHoraDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }
        public string HoraDesde { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP_FechaHoraHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }
        public string HoraHasta { get; set; }
        public TipoDeServicio TipoDeServicio { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaHasta != DateTime.MinValue && FechaHasta < FechaDesde)
            {
                yield return new ValidationResult(string.Format(Textos.MasGrandeQue, Textos.FechaFin, Textos.FechaInicio), new[] { "FechaHasta" });
            }
        }
    }
}