using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LibroMovimientosExistenciaGranosDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material_DescripcionCorta")]
        public string MaterialDescripcionCorta { get; set; }
        public int MaterialId { get; set; }
        public int MaterialDescripcionCortaId { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaterialId == 0 && MaterialDescripcionCortaId == 0 && string.IsNullOrEmpty(MaterialDescripcionCorta))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.Material), new[] { "MaterialDesc" });
            }

            if (MaterialId == 0 && MaterialDescripcionCortaId == 0 && !string.IsNullOrEmpty(MaterialDescripcionCorta))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.Material), new[] { "MaterialDescripcionCorta" });
            }

            if (MaterialId != 0 && MaterialDescripcionCortaId != 0)
            {
                yield return new ValidationResult(string.Format(Textos.Error_LibroOnccaMaterial, Textos.Material), new[] { "MaterialDesc" });
                yield return new ValidationResult("", new[] { "MaterialDescripcionCorta" });
            }

        }
    }
}