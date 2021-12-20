using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class InhabilitacionCamionDto : IValidatableObject
    {
        public int Id { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        //[RegularExpression(@"[A-Z]{3}[0-9]{3}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InhabilitacionChofer_Motivo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Motivo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "JustificacionCambioInhabilitacionChofer")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Comentario { get; set; }
        public int CentroId { get; set; }

        public string NombreUsuarioResponsable { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Adjunto")]
        public String ArchivoAdjunto { get; set; }

        public IList<AdjuntoDto> Adjuntos { set; get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaHasta != DateTime.MinValue && FechaHasta < FechaDesde)
            {
                yield return new ValidationResult(string.Format(Textos.MasGrandeQue, Textos.FechaFin, Textos.FechaInicio), new[] { "FechaHasta" });
            }
        }
    }
}
