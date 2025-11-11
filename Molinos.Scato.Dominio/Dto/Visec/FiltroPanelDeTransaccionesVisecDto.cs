using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class FiltroPanelDeTransaccionesVisecDto : IValidatableObject
    {
        [Display(ResourceType = typeof(Textos), Name = "Estado")]
        public EstadoTransmisionAVisec? EstadoTransmisionAVisec { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Pesada_NumeroDocumento")]
        public string NumeroDocumento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDesde2")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta2")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime? FechaHasta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaHasta != DateTime.MinValue && FechaHasta < FechaDesde)
            {
                yield return new ValidationResult(string.Format(Textos.MasGrandeQue, Textos.FechaHasta, Textos.FechaInicio), new[] { "FechaHasta2" });
            }
        }
    }
}