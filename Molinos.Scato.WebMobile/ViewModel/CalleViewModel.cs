using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class CalleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Camiones { get; set; }
    }
}