using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VinedoDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "VinedoPropio_NumeroINV")]
        public string NumeroINV { get; set; }
    }
}