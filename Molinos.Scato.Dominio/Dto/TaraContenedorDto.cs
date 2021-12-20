using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TaraContenedorDto
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CodigoContenedor")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string CodigoContenedor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(20, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PesoTara")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PesoTara { get; set; }
    }
}