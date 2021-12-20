using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TarjetaRangoDto : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{5,5}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo5Digitos")]
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        public string Codigo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "TarjetaRango_ValidoDesde")]
        public DateTime ValidoDesde { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "TarjetaRango_ValidoHasta")]
        public DateTime ValidoHasta { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{5,5}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo5Digitos")]
        [Display(ResourceType = typeof(Textos), Name = "TarjetaRango_RangoDesde")]
        public string RangoDesde { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{5,5}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo5Digitos")]
        [Display(ResourceType = typeof(Textos), Name = "TarjetaRango_RangoHasta")]
        public string RangoHasta { get; set; }
        public int CentroId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.Compare(RangoHasta, RangoDesde, StringComparison.Ordinal) < 0 )
            {
                yield return new ValidationResult(Textos.TarjetaRango_ErrorRangos, new[] { "RangoHasta" });
            }
            if (ValidoHasta < ValidoDesde)
            {
                yield return new ValidationResult(Textos.TarjetaRango_ErrorFechas, new[] { "ValidoHasta" });
            }
        }
    }
}
