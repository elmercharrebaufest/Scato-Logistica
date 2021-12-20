using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VariedadDto
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material_CodigoINV")]
        public string NumeroINV { get; set; }
    }
}