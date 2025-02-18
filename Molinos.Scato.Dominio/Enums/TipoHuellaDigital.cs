using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoHuellaDigital : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Huella Manual")]
        HuellaManual = 2,
        [Display(ResourceType = typeof(Textos), Name = "Recorrido")]
        Recorrido = 1,
    }
}