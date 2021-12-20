using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DescuentoDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descuento_ValorHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal ValorHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descuento_PorcentajeDescuento")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal PorcentajeDescuento { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descuento_PorcentajeAuditoriaCamara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"^[0-9]*(?:\,[0-9]*)?$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public decimal PorcentajeEnvioCamaraAuditoria { get; set; }
        public bool MercadoATermino { get; set; }

        public bool _destroy { get; set; }
        public bool EsNuevo { get; set; }
        public int CaracteristicaDeCalidadId { get; set; }
    }
}
