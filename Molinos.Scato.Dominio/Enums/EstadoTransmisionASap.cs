using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum EstadoTransmisionASap
    {
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Correcta")]
        Correcto = 0,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Pendiente")]
        Pendiente = 1,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Error")]
        Error = 2,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASap_Cancelada")]
        Cancelada = 3
    }
}
