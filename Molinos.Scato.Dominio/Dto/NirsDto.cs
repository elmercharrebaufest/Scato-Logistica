using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class NirsDto
    {
        public int Id { get; set; }
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }
        [StringLength(8, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "DescripcionCorta")]
        public string DescripcionCorta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        public string Codigo { get; set; }
        public Modalidad Modalidad { get; set; }
        public int CentroId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Balanza_PuestoDeTrabajo")]
        [StringLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string PuestoDeTrabajo { get; set; }
    }
}
