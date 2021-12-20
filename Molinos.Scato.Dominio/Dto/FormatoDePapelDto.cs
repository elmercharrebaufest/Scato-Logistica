using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class FormatoDePapelDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }

        public int Ancho { get; set; }
        public int Alto { get; set; }
    }
}
