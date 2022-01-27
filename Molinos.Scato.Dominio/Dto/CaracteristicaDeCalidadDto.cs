using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CaracteristicaDeCalidadDto : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [MaxLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string DescripcionCorta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_UnidadDeMedida")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string UnidadDeMedida { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_DescuentoEnPorcentaje")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public FormulaDescuento DescuentoEnPorcentaje { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ValorMinomo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal CaladoMinimo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ValorMAximo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal CaladoMaximo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ToleranciaSinAnalisis")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ToleranciaSinAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoSAP")]
        [MaxLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsProteinaAlta")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ValorProteina { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsProteinaMedia")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ValorProteinaMedia { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_TipoDeAnalisis")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoAnalisis Analisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_Ensayo")]
        [MaxLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^[FQC]\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_TipoEnsayo")]
        public string Ensayo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_Obligatorio")]
        public bool CargaEnCalado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_Obligatorio")]
        public bool Obligatorio { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_NoObservableEnCalado")]
        public bool NoObservableEnCalado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_InternoPorObservados")]
        public bool InternoPorObservados { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_InspeccionDeCamionesVacios")]
        public bool InspeccionDeCamionesVacios { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsModificable")]
        public bool EsModificable { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsHumedad")]
        public bool EsHumedad { get { return TipoCaracteristica == CaracteristicasCalidad.EsHumedad; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsTenorAzucarino")]
        public bool EsTenorAzucarino { get { return TipoCaracteristica == CaracteristicasCalidad.EsTenorAzucarino; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsEstadoSanitario")]
        public bool EsEstadoSanitario { get { return TipoCaracteristica == CaracteristicasCalidad.EsEstadoSanitario; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsCalidadUva")]
        public bool EsCalidadUva { get { return TipoCaracteristica == CaracteristicasCalidad.EsCalidadUva; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsCuerposExtraños")]
        public bool EsCuerposExtranos { get { return TipoCaracteristica == CaracteristicasCalidad.EsCuerposExtranos; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsGranosVerdes")]
        public bool EsGranosVerdes { get { return TipoCaracteristica == CaracteristicasCalidad.EsGranosVerdes; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsDañado")]
        public bool EsGranosDañados { get { return TipoCaracteristica == CaracteristicasCalidad.EsGranosDañados; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsMermaVolatil")]
        public bool EsMermaVolatil { get { return TipoCaracteristica == CaracteristicasCalidad.EsMermaVolatil; } }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsProteina")]
        public bool EsProteina { get { return TipoCaracteristica == CaracteristicasCalidad.EsProteina; } }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsInsectosVivos")]
        public bool EsInsectosVivos { get { return TipoCaracteristica == CaracteristicasCalidad.EsInsectosVivos; } }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_PrioridadEnCalado")]
        [Range(0, 99, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public int? PrioridadEnCalado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_NoAceptarSiSeDefineUnValor")]
        public bool NoAceptarSiSeDefineUnValor { get; set; }
        public string CodigoCamara { get; set; }

        public List<DescuentoDto> DescuentosDto { get; set; }

        public bool SeEnviaACamara { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EnvioACamara")]
        public EnvioACamara SituacionEnvioACamara { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_SiSuperaValorCamara")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? SiSuperaValorCamara { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "TipoCaracteristica")]
        public CaracteristicasCalidad TipoCaracteristica { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ValorEspecialMinimo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ValorEspecialMinimo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ValorEspecialMaximo")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ValorEspecialMaximo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsAutomatizable")]
        public bool EsAutomatizable { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_TipoDeDispositivo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public TipoDispositivo Dispositivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Nirs_NombreCalidad")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NombreNirs { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CaladoMaximo < CaladoMinimo)
            {
                yield return new ValidationResult(Textos.Error_Invalido, new[] { "CaladoMaximo" });
            }

            if (ToleranciaSinAnalisis < CaladoMinimo || ToleranciaSinAnalisis > CaladoMaximo)
            {
                yield return new ValidationResult(Textos.Error_Invalido, new[] { "ToleranciaSinAnalisis" });
            }
        }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_IntervaloDeAnalisis")]
        public bool IntervaloDeAnalisis { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EnviaASap")]
        public bool EnviaASap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_ToleranciaSinMensaje")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999999999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public decimal? ToleranciaSinMensaje { get; set; }

    }
}
