using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoVariedadPorMaterialDto : AuditoriaBaseDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Variedad")]
        public string TipoVariedadDescripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int TipoVariedadId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string ColorFondo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string ColorTexto { get; set; }
        public string VariedadDescripcion { get; set; }
    }
}