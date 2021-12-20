using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TecnologiaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Chofer_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Nombre { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(2, MinimumLength = 2, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Codigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Empresa")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }

    }
}