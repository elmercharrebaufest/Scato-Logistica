using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConversionCaracteristicaDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public int CamaraId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string CamaraDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Caracteristica")]
        public int? CaracteristicaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Caracteristica")]
        public string CaracteristicaDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TablaConversion_CodigoCamara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoCamara { get; set; }
    }
}

