using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Enums
{
    public enum MedioDePago : int
    {
        [Display(ResourceType = typeof(Textos), Name = "Efectivo")]
        Manual = 0,
        
        [Display(ResourceType = typeof(Textos), Name = "MercadoPago")]
        MercadoPago = 1,

        [Display(ResourceType = typeof(Textos), Name = "Digital")]
        Digital = 2
    }
}
