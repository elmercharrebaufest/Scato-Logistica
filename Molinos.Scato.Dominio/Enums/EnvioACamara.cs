using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum EnvioACamara
    {
        [Display(ResourceType = typeof(Textos), Name = "Siempre")]
        Siempre = 0,
        [Display(ResourceType = typeof(Textos), Name = "Nunca")]
        Nunca = 1,
        [Display(ResourceType = typeof(Textos), Name = "EnCoordinacion")]
        EnCoordinacion = 2,
        [Display(ResourceType = typeof(Textos), Name = "SiempreSiSuperaValorCamara")]
        SiempreSiSuperaValorCamara = 3
    }
}
