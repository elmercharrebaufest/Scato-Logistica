using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialPorCentroDto
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }
        public string MaterialDesc { get; set; }

        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_AlmacenPred")]
        public int? AlmacenPredId { get; set; }
        public string AlmacenPredDesc { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_RangoPorcentaje")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Material_AnalisisInterno")]
        public int? AnalisisInterno { get; set; }

        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Display(ResourceType = typeof(Textos), Name = "Material_PorcentajeMustraAuditoria")]
        public decimal? PorcentajeMuestraAuditoria { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CamaraPred")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int? CamaraId { get; set; }
        public string CamaraDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_CorrespondeDescarga")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public bool CorrespondeDescarga { get; set; }

        public bool RequiereTecnologia { get; set; }

        public bool MaterialDeTerceros { get; set; }

        public string Almacenes { get; set; }

        public string AlmacenesEliminados { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_ImprimeReciboMunicipal")]
        public bool ImprimeReciboMunicipal { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_NoValidaCG")]
        public bool NoValidaCG { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EPAStockPorCorte")]
        public int? EpaStockPorCorte { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_MostrarEnWebMobile")]
        public bool MostrarEnWebMobile { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_DescripcionWebMobile")]
        public string DescripcionWebMobile { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "OrdenDescripcionMaterialMobile")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public int? Orden { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "IgnoraContingencia")]
        public bool IgnoraContingencia { get; set; }
    }
}
