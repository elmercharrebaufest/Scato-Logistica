using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoArchivo : int
    {
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCamiones")]
        ListadoDeCamiones  = 0,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDePesadas")]
        ListadoDePesadas  = 1,
    }
}
