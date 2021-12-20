using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class OrdenEntrePlantasDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDEntrePlantas_Nro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Numero { get; set; }

        [FechaAntigua]
        [Display(ResourceType = typeof(Textos), Name = "Fecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        public string MaterialCodigoSap { get; set; }
        public bool MaterialPideLote { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }
        public int TransportistaId { get; set; }
        public bool EsTransportista { get; set; }
        public string TransportistaCuit { get; set; }
        public ChoferDto Chofer { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CentroDestino { get; set; }
        public int CentroDestinoId { get; set; }
        public string CentroDestinoCodigoSap { get; set; }
        public string CentroDestinoCuit { get; set; }
        public string CentroDestinoLocalidad { get; set; }
        public string CentroDestinoDireccion { get; set; }
        public string CentroDestinoProvincia { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        public string TipoComercial { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }

        public int RecorridoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodigoAnexo")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoAnexo { get; set; }
        public bool RequiereAnexoInase { get; set; }

        public int? PesoNetoOrigen { get; set; }

        public string DocumentoInternoSap { get; set; }

        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_KmRecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? KmRecorrer { get; set; }

        public string KmARecorrer { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Numero))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.OrdenDeDescarga), new[] { "OrdenDeDescarga" });
            }
            if (TipoDeWorkflow == TipoDeWorkflow.Ingreso && string.IsNullOrEmpty(CodigoAnexo) && RequiereAnexoInase)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_CodigoAnexo), new[] { "CodigoAnexo" });
            }
        }
    }
}