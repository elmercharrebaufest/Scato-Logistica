using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum TipoStockBines : int
    {
        Centro = 0,
        Productor = 1,
        [Display(ResourceType = typeof(Textos), Name = "Filtrar_VinedoPropio")]
        VinedoPropio = 2
    }
}
