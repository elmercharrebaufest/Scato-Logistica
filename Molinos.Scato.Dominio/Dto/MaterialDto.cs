using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoSAP")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [StringLength(15, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoEspecie")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? CodigoEspecie { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_UnidadDeMedida")]
        [StringLength(3, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string UnidadDeMedidad { get; set; }

        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Material_FactorConversion")]
        public decimal? FactorConversion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_TipoDeGrano")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? TipoDeGrano { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoONCCA")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [StringLength(6, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoONCCA { get; set; }

        //[Range(0, 100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_RangoPorcentaje")]
        //[RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        //[Display(ResourceType = typeof(Textos), Name = "Material_AnalisisInterno")]
        //public int? AnalisisInterno { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CamaraPred")]
        public string CamaraDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_ReqNumeroTropa")]
        public bool RequiereNumeroTropa { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_BinPallet")]
        public bool UsaBinPallet { get; set; }

        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Material_Peso")]
        public decimal? Peso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Clase")]
        public ClaseBin? Clase { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsUva")]
        public bool EsUva { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Variedad")]
        public int? VariedadId { get; set; }

        public string VariedadDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Commodity")]
        public bool Commodity { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_AlmacenOrigen")]
        public int? AlmacenOrigenId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_AlmacenOrigen")]
        public string AlmacenOrigenDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsCosecha")]
        public bool EsCosecha { get; set; }

        //[Display(ResourceType = typeof(Textos), Name = "Material_Cosecha")]
        //[StringLength(5, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        //public string Cosecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AnioDesde")]
        public int? VigenciaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AnioHasta")]
        public int? VigenciaHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Activo")]
        public bool Activo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Lote")]
        public bool Lote { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Contrato")]
        public bool Contrato { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_RequiereAnexoInase")]
        public bool RequiereAnexoInase { get; set; }

        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Material_PesoTeorico")]
        public decimal? PesoTeoricoSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoProducto")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NirsCodigoProducto { get; set; }

        public bool Oleico { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsGrano")]
        public bool EsGrano { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OrdenDescripcionMaterialMobile")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? Orden { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsInsumo")]
        public bool EsInsumo { get; set; }

        public bool EsAsignableCalle { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_ColorFondo")]
        public string ColorFondo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_ColorTexto")]
        public string ColorTexto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_EsDerivadoGranario")]
        public bool EsDerivadoGranario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoGranoPadre")]
        public int? CodigoGranoPadre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_TipoEmbalaje")]
        public int? TipoEmbalajeId { get; set; }
    }
}