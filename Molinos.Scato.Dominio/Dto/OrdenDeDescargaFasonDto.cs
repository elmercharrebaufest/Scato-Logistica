using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenDeDescargaFasonDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescarga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Numero { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescargaFason_NumeroRemito")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string NumeroRemito { get; set; }

        [FechaAntigua]
        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescargaFason_FechaOD")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaOD { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenCargaFAS_Cliente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Cliente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ClienteId { get; set; }
        public string ClienteCodigoSap { get; set; }
        public string ClienteCuit { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Procedencia")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Procedencia { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }
        public int ProcedenciaId { get; set; }
        public string ProcedenciaCodigoSap { get; set; }
        public int ProvinciaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista")]
        public string Transportista { get; set; }

        public int TransportistaId { get; set; }

        public bool EsTransportista { get; set; }

        public ChoferDto Chofer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public string TipoComercial { get; set; }

        public string TipoComercialSentido { get; set; }

        public string TipoComercialCodigoSap { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Material")]
        public string Material { get; set; }

        public string MaterialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoTaraOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoTaraOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoBrutoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 99999)]
        public int? PesoBrutoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoNetoOrigen")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int PesoNetoOrigen { get; set; }

        public TipoDeWorkflow TipoDeWorkflow { get; set; }
        public int RecorridoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }
        public int TipoVehiculoInt
        {
            get { return (int)TipoVehiculo; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Numero))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.OrdenDeDescarga), new[] { "OrdenDeDescarga" });
            }
        }
    }
}