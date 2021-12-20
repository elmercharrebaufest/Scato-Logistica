using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ExportacionDeArchivosDto : IValidatableObject
    {
       
        [Display(ResourceType = typeof(Textos), Name = "FechaEgresoDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }
       
        [Display(ResourceType = typeof(Textos), Name = "FechaEgresoHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        public string Centros { get; set; }
        public string Materiales { get; set; }
        public string TiposComerciales { get; set; }

        //Para listado de calidades que tiene un solo material
        public string Material { get; set; }
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoArchivo")]
        public TipoArchivo TipoArchivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "IncluirRechazados")]
        public bool IncluirRechazados { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaDesde > FechaHasta)
            {
                yield return new ValidationResult(Textos.Error_FechaMayor + Textos.FechaDesde, new[] { "FechaHasta" });
            }
        }
    }
}
