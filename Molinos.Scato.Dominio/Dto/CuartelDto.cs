using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CuartelDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_NumeroINV")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(10, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Codigo { get; set; }
        public int VinedoPropioId { get; set; }
        public string VinedoPropio { get; set; }
        public bool Activo { get; set; }
    }
}
