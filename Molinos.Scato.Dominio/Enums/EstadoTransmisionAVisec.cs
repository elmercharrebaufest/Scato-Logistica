using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum EstadoTransmisionAVisec
    {
        [Display(ResourceType = typeof(Textos), Name = "TransmisionVisec_Estado_Pendiente")]
        Pendiente = 0,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionVisec_Estado_EnviadoAVisec")]
        EnviadoAVisec = 1,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionVisec_Estado_Error")]
        Error = 2,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionVisec_Estado_Finalizado")]
        Finalizado = 3,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionVisec_Estado_ErrorVisec")]
        ErrorVisec = 4,
    }
}
