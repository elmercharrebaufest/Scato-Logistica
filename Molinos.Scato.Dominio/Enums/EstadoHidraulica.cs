using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum EstadoHidraulica
    {
        [Display(ResourceType = typeof(Textos), Name = "EstadoHidraulica_Inhabilitado")]
        Inhabilitado = 0,
        [Display(ResourceType = typeof(Textos), Name = "EstadoHidraulica_Disponible")]
        Disponible = 1,
        [Display(ResourceType = typeof(Textos), Name = "EstadoHidraulica_Llamando")]
        Llamando = 2,
        [Display(ResourceType = typeof(Textos), Name = "EstadoHidraulica_Ocupado")]
        Ocupado = 3,
    }
}
