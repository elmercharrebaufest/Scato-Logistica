using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum TipoDeServicio : int
    {
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP")]
        Sap = 0,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransMonsanto")]
        Monsanto = 1,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransaccionesSapCupo")]
        Cupo = 2,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlExcepcionPagoMunicipal")]
        PanelDeControlPagoMunicipal = 3,
    }
}
