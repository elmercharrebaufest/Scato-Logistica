using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoDeProteina : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Proteina_Todos")]
        Todos = 0,
        [Display(ResourceType = typeof(Textos), Name = "Proteina_Alta")]
        Alta = 1,
        [Display(ResourceType = typeof(Textos), Name = "Proteina_Baja")]
        Baja = 2,
    }
}