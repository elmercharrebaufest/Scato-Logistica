using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum TipoPesada
    {
        //IMPORTANTE: Cada vez que se agregue un dato se debe actualizar manualmente en la tabla correspondiente.
        [Display(ResourceType = typeof(Textos), Name = "Bruto")]
        Bruto,
        [Display(ResourceType = typeof(Textos), Name = "Tara")]
        Tara
    }
}
