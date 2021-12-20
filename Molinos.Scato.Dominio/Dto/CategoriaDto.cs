using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CategoriaDto
    {

        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Clasificacion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Clasificacion { get; set; }
    }
}