using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RemitoBodegaUvaDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_OrdenDeCompra")]
        [RegularExpression(@"\d{10,10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo10Digitos")]
        public string OrdenDeCompra { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Remito_NroRemito")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{4,4}\-\d{8,8}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo12Digitos")]
        public string NroRemito { get; set; }

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
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_MarcaCamion")]
        public string MarcaCamion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_ModeloCamion")]
        public string ModeloCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoVehiculoBodegaId { get; set; }
        public string TipoVehiculoBodega { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public string TipoComercial { get; set; }
        public string TipoComercialSentido { get; set; }
        public string TipoComercialCodigoSap { get; set; }
        public TipoUva TipoComercialEsParaUva { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_TipoCosecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoCosecha? TipoCosecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RemitoBodega_Vinedo")]
        public string Vinedo { get; set; }
        public int VinedoId { get; set; }
        public string VinedoVinatero { get; set; }
        public string VinedoINV { get; set; }
        public string VinedoCuit { get; set; }
        public string VinedoIIBB { get; set; }
        public string VinedoCentroOperativo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialIdYPosicion { get; set; }
        public int MaterialId { get; set; }
        public string Posicion { get; set; }
        public string Material { get; set; }
        public string MaterialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Cosecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Cosecha { get; set; }

        public int VariedadId { get; set; }
        public string Variedad { get; set; }
        public int RecorridoId { get; set; }

        public IList<DescargaDeBinesDto> DescargasDeBines { get; set; }

        public IList<CargaDeBinesDto> CargasDeBines { get; set; }
        
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

        //Se utiliza en CoordinacionController
        public Guid InstanciaWorkflow { get; set; }

        //TransmisionSapIngresoBodegas
        public string EsPropiaPesTercerosT { get; set; }
        public string VinedoSubZona { get; set; }
        public string VinedoZona { get; set; }
        public string VinedoCalidad { get; set; }
        public int? PesoNetoBodega { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Patente))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_PatenteCamion), new[] { "Patente" });
            }
        }
    }
}