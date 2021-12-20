using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoEstado : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Todos")]
        Todos = 0,
        [Display(ResourceType = typeof(Textos), Name = "Rechazados")]
        Si = 1,
        [Display(ResourceType = typeof(Textos), Name = "NoRechazados")]
        No = 2,
    }
}
