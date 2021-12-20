using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CasilleroDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Casillero")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Numero { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Casillero_Capacidad")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Capacidad { get; set; }
    }
}
