using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LogHumedimetroDto: IValidatableObject
    {
        [Display(ResourceType = typeof (Textos), Name = "FechaDesde2")] 
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta2")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public int? CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Humedimetro")]
        public int? HumedimetroId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaDesde > FechaHasta)
            {
                yield return new ValidationResult(string.Format(Textos.Error_FechaMayor + Textos.FechaDesde2,Textos.FechaHasta2), new[] {"FechaHasta2"});
            }
        }
    }
}
