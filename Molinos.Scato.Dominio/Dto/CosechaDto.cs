using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CosechaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Cosecha")]
        [Required(ErrorMessageResourceType = typeof (Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EPApesoDescontado")]
        public bool EpaPesoDescontado { get; set; }
    }
}
