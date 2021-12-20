using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ModificarDocumentoDeIngresoDto : IValidatableObject
    {
        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "NumeroTarjeta")]
        public string NumeroDeTarjeta { get; set; }
        public bool FiltrarPorTarjeta { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FiltrarPorTarjeta && string.IsNullOrEmpty(NumeroDeTarjeta))
            {
                yield return
                    new ValidationResult(string.Format(Textos.Error_Requerido, Textos.NumeroTarjeta), new[] { "NumeroDeTarjeta" });
            }
            if (!FiltrarPorTarjeta && string.IsNullOrEmpty(NumeroDocumentoIngreso))
            {
                var valresult = new ValidationResult(string.Format(Textos.Error_Requerido, Textos.NumeroDocumentoIngreso), new[] { "NumeroDocumentoIngreso" });
                yield return valresult;
            }
        }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TitularCartaPorte")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string TitularCartaPorte { get; set; }
        public int TitularCartaPorteId { get; set; }
        public DateTime? FechaDesdeCP { get; set; }
        public DateTime? FechaHastaCP { get; set; }
        public DateTime? FechaVto { get; set; }
        public DateTime? FechaDigitalizacion { get; set; }
        public string Corredor { get; set; }
        public int CorredorId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "MaterialPorWorkflow_Material")]
        public string MaterialDesc { get; set; }

        public int MaterialId { get; set; }
        public DateTime? FechaTaraDesdeCP { get; set; }
        public DateTime? FechaTaraHastaCP { get; set; }
    }
}
