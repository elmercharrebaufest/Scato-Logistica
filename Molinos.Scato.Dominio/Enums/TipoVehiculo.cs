using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoVehiculo : int
    {
        Camiones = -1,
        //IMPORTANTE: Cada vez que se agregue un dato se debe actualizar manualmente en la tabla correspondiente.
        Camión = 0,
        Tren = 1,
        [Display(Name = "Bitren B(60)")]
        Bitren = 2,
        [Display(Name = "Camión C(55,5)")]
        CamiónC = 3,
        [Display(Name = "Camión D(52,5)")]
        CamiónD = 4,
        [Display(Name = "Camión E(49,5)")]
        CamiónE = 5,
        Vapor = 6
    }
}