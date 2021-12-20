using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RemitoBodegaVinoDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Remito_NroRemito")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{4,4}\-\d{8,8}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo12Digitos")]
        public string NroRemito { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_OrdenDeCompra")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{10,10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo10Digitos")]
        public string OrdenDeCompra { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public string TipoComercial { get; set; }
        public string TipoComercialSentido { get; set; }
        public string TipoComercialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public string Proveedor { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorCodigoSap { get; set; }
        public string ProveedorCuil { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista")]
        public string Transportista { get; set; }
        public int TransportistaId { get; set; }
        public string TransportistaCUIT { get; set; }
        public bool EsTransportista { get; set; }
        
        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialIdYPosicion { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public string MaterialCodigoSap { get; set; }
        public string Posicion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_MarcaCamion")]
        public string MarcaCamion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_ModeloCamion")]
        public string ModeloCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoVehiculoBodegaId { get; set; }
        public string TipoVehiculoBodega { get; set; }

        public int VariedadId { get; set; }
        public string Variedad { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }
        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoTaraOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoTaraOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoBrutoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoBrutoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoNetoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoNetoOrigen { get; set; }

        public int RecorridoId { get; set; }
        //Se utiliza en CoordinacionController
        public Guid InstanciaWorkflow { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Patente))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_PatenteCamion), new[] { "Patente" });
            }
        }
    }
}