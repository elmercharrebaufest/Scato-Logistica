using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoUva : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Default_NoAplica")]
        NoAplica = 0,
        [Display(ResourceType = typeof(Textos), Name = "Propia")]
        Propia = 1,
        [Display(ResourceType = typeof(Textos), Name = "Terceros")]
        Terceros = 2
    }
}
