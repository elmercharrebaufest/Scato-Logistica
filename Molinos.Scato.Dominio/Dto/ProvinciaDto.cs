using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ProvinciaDto
    {
        public int Id { get; set; }
        public int CodigoAfip { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        public PaisDto Pais { get; set; }
    }
}
