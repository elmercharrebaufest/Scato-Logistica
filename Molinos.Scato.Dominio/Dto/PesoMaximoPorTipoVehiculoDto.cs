using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PesoMaximoPorTipoVehiculoDto : IValidatableObject
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public TipoVehiculo TipoVehiculo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PesoMaxIngreso")]
        public int PesoMaxIngreso { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PesoMaxEgreso")]
        public int PesoMaxEgreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoNetoMaxPlanta")]
        public int? PesoNetoMaxPlanta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoNetoMinimo")]
        public int? PesoNetoMinimo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Activo")]
        public bool Activo { get; set; }

        public int CentroId { get; set; }
        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if (PesoMaxIngreso < 0 || PesoMaxIngreso > int.MaxValue)
            {
                yield return new ValidationResult(string.Format(Textos.Error_NumericoYRango,Textos.PesoMaxIngreso), new[] { "PesoMaxIngreso" });
            }
            if (PesoMaxEgreso < 0 || PesoMaxEgreso > int.MaxValue)
            {
                yield return new ValidationResult(string.Format(Textos.Error_NumericoYRango, Textos.PesoMaxEgreso), new[] { "PesoMaxEgreso" });
            }
            if (PesoNetoMaxPlanta != null && (PesoNetoMaxPlanta < 0 || PesoNetoMaxPlanta > int.MaxValue))
            {
                yield return new ValidationResult(string.Format(Textos.Error_NumericoYRango, Textos.PesoNetoMaxPlanta), new[] { "PesoNetoMaxPlanta" });
            }
            if (PesoNetoMinimo != null && (PesoNetoMinimo < 0 || PesoNetoMinimo > int.MaxValue))
            {
                yield return new ValidationResult(string.Format(Textos.Error_NumericoYRango, Textos.PesoNetoMinimo), new[] { "PesoNetoMinimo" });
            }
        }
    }
}
