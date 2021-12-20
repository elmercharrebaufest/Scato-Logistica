using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LocalidadDto
    {
        public int Id { get; set; }
        public string CodigoAfip { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Descripcion { get; set; }
        
        public int ProvinciaId { get; set; }
        public string ProvinciaDesc { get; set; }
    }
}
