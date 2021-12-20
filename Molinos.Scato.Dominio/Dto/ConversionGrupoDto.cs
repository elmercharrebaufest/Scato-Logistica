using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConversionGrupoDto
    {
        public int? Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public int CamaraId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string CamaraDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TablaConversion_CodigoCamara")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoSegunCamara { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }
    }
}

