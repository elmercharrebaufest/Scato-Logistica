using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum FiltroEntregador : int
    {
        [Display(ResourceType = typeof(Textos), Name = "FiltroEntregador_Todos")]
        Todos = 0,
        [Display(ResourceType = typeof(Textos), Name = "FiltroEntregador_ConEntregador")]
        ConEntregador = 1,
        [Display(ResourceType = typeof(Textos), Name = "FiltroEntregador_SinEntregador")]
        SinEntregador = 2,
    }
}
