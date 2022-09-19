using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoCalle : int
    {
        [Display(ResourceType = typeof(Textos), Name = "TipoCallePlayaInterna")]
        PlayaInterna = 0,
        [Display(ResourceType = typeof(Textos), Name = "TipoCallePreCalado")]
        PreCalado = 1,
        [Display(ResourceType = typeof(Textos), Name = "TipoCallePostCalado")]
        PostCalado = 2,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleReCalado")]
        ReCalado = 3,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleCalado")]
        Calado = 4,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleRechazadosDemorados")]
        RechazadosDemorados = 5,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleNoGranos")]
        NoGranos = 6,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleCircular")]
        Circular = 7,
        [Display(ResourceType = typeof(Textos), Name = "TipoCallePlantaNoGranos")]
        PlantaNoGranos = 8,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleEnTransito")]
        EnTransito = 9,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleSalidaNoGranos")]
        SalidaNoGranos = 10,
        [Display(ResourceType = typeof(Textos), Name = "TipoCalleEsperaAduanaNoGranos")]
        EsperaAduanaNoGranos = 11

    }
}
