using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CamaraDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string DescripcionCorta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara_CodigoSAP")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string CodigoSAP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_FormatoCamara")]
        public CamaraFormatoDeArchivo FormatoDeArchivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara_Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(200, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Email { get; set; }


    }
}
