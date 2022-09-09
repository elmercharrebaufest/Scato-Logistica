using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoCalle : int
    {
        PlayaInterna = 0,
        PreCalado = 1,
        PostCalado = 2,
        ReCalado = 3,
        Calado = 4,
        RechazadosDemorados = 5,
        NoGranos = 6,
        Circular = 7,
        PlantaNoGranos = 8,
        EnTransito = 9,
        SalidaNoGranos = 10
    }
}
