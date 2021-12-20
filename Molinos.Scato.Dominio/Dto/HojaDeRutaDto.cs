using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HojaDeRutaDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Numero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Numero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }
        public int TransportistaId { get; set; }
        public bool EsTransportista { get; set; }
        public string TransportistaCuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public bool EsExtranjero { get; set; }

        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public string TipoComercial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        public string MaterialCodigoSap { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Numero))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.Numero), new[] { "Numero" });
            }
        }
    }
}