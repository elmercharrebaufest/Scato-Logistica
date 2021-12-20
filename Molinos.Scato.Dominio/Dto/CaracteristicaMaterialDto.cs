using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CaracteristicaMaterialDto
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
    }
}