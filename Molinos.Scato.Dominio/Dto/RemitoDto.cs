using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RemitoDto : IValidatableObject
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDeDescarga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string OrdenDeDescarga { get; set; }

        [FechaAntigua]
        [Display(ResourceType = typeof(Textos), Name = "IngresoRemito_FechaOD")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaOD { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Remito")]
        [StringLength(13, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Remito { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoComercialId { get; set; }
        public string TipoComercial { get; set; }
        public string TipoComercialCodigoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Origen { get; set; }
        public int OrigenId { get; set; }
        public string OrigenCodigoSap { get; set; }
        public string OrigenMail { get; set; }
        public bool OrigenEnviaMailEnPesada { get; set; }
        public bool OrigenEnviaMailEnAnalisis { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_EsExtranjero")]
        public Boolean EsExtranjero { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Transportista")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Transportista { get; set; }
        public string TransportistaCodigoSap { get; set; }
        public string TransportistaCuit { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TransportistaId { get; set; }
        public bool EsTransportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteCamion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string PatenteCamion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        public ChoferDto Chofer { get; set; }

        public TipoDeWorkflow TipoDeWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
        public bool MaterialPideContrato { get; set; }
        public string MaterialCodigoSap { get; set; }

        public bool EsRemitoProveedor { get; set; }

        public string DocLegalRemito { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoTaraOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoTaraOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoBrutoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoBrutoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_PesoNetoOrigen")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? PesoNetoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_AcuerdoMarco")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string AcuerdoMarco { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodigoAnexo")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoAnexo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_KmRecorrer")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? KmRecorrer { get; set; }

        public bool RequiereAnexoInase { get; set; }

        public int RecorridoId { get; set; }

        public ProveedorDto ProveedorOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Cosecha")]
        [StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Cosecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_CodEstab")]
        [StringLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodEstab { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Procedencia")]
        public string Procedencia { get; set; }
        public int ProcedenciaId { get; set; }
        public string ProcedenciaCodigoSap { get; set; }
        public string ProvinciaCodigoSap { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public int? Calle_Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_TipoVehiculo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoVehiculo TipoVehiculo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CartaPorte_Almacen")]
        public int? Almacen_Id { get; set; }
        public int TipoVehiculoInt
        {
            get { return (int)TipoVehiculo; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoDeWorkflow == TipoDeWorkflow.Ingreso && string.IsNullOrEmpty(CodigoAnexo) && RequiereAnexoInase)
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.CartaPorte_CodigoAnexo), new[] { "CodigoAnexo" });
            }

            if (string.IsNullOrEmpty(OrdenDeDescarga))
            {
                yield return new ValidationResult(string.Format(Textos.Error_Requerido, Textos.OrdenDeDescarga), new[] { "OrdenDeDescarga" });
            }        
        }
    }
}