using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class CalleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Range(1, int.MaxValue)]
        public int Camiones { get; set; }
    }
}