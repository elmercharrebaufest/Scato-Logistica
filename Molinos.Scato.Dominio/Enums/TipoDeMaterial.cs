using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoMaterial : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Material_Todos")]
        Todos = 0,
        [Display(ResourceType = typeof(Textos), Name = "Material_NoGranos")]
        NoGranos = 1,
        [Display(ResourceType = typeof(Textos), Name = "Material_Granos")]
        Granos = 2,
    }
}