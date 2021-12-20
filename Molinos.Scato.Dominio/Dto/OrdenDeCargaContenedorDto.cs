using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenDeCargaContenedorDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDeCargaContenedor_Nro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string OrdenDeCargaContenedor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }
        public int TransportistaId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public bool EsExtranjero { get; set; }
        public bool EsTransportista { get; set; }
        public string TransportistaCuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public string TipoComercial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }

        public string Material { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        public string MaterialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destino { get; set; }
        public string DestinoCodigoSap { get; set; }
        public int DestinoId { get; set; }
        
        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ContenedorEntrada")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ContenedorEntradaId { get; set; }
        public int PesoContenedorEntrada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ContenedorSalida")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ContenedorSalidaId { get; set; }
        public int PesoContenedorSalida { get; set; }

        public int RecorridoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(OrdenDeCargaContenedor))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.OrdenDeDescarga), new[] { "OrdenDeDescarga" });
            }
        }
    }
}