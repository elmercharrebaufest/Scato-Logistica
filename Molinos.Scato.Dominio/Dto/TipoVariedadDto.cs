using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoVariedadDto : AuditoriaBaseDto
    {
        public int Id { get; set; }

        [StringLength(10)]
        [Display(ResourceType = typeof(Textos), Name = "Codigo_Interno")]
        public string Codigo { get; set; }

        [Required]
        [StringLength(100)]
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        public string ColorTexto { get; set; }

        public string ColorFondo { get; set; }

        public bool EstaEnAutomatismo { get; set; }
    }
}