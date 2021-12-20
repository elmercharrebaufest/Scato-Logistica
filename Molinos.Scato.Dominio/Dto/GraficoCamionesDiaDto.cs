using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class GraficoCamionesDiaDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string MaterialDescripcion { get; set; }
        public int MaterialId { get; set; }

        public int[] CamionesDia { get; set; }
    }
}