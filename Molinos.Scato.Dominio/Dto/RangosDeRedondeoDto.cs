using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RangosDeRedondeoDto
    {
        public int Id { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo_ValorDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal ValorDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo_ValorHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal ValorHasta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo_ValorRedondeado")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal ValorRedondeado { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo_MaterialPorCentroId")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialPorCentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo_MaterialPorCentroId")]
        public string MaterialPorCentroDescripcion { get; set; }
    }
}
