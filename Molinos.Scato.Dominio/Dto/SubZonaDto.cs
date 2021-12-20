using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class SubZonaDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        public ZonaDto Zona { get; set; }
    }
}
