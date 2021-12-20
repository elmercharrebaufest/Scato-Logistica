using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    //IMPORTANTE: Cada vez que se agregue un dato se debe actualizar manualmente en la tabla correspondiente.
    public enum FuncionSAP : int
    {
        [Display(ResourceType = typeof(Textos), Name = "IngresosPorCompraDeGranos")]
        IngresosPorCompraDeGranos = 0,
        [Display(ResourceType = typeof(Textos), Name = "SalidaDeOrigenEnRedespachos")]
        SalidaDeOrigenEnRedespachos = 1,
        [Display(ResourceType = typeof(Textos), Name = "LlegadaADestinosEnRedespachos")]
        LlegadaADestinosEnRedespachos = 2,
        [Display(ResourceType = typeof(Textos), Name = "AjusteDeDiferencias")]
        AjusteDeDiferencias = 3,
        [Display(ResourceType = typeof(Textos), Name = "EgresosNoProductivos")]
        EgresosMaterialNoProductivo = 4,
        [Display(ResourceType = typeof(Textos), Name = "IngresosEgresosFazones")]
        IngresosEgresosFazones = 5,
        [Display(ResourceType = typeof(Textos), Name = "PesaNeto_Trasmision")]
        PesaNeto = 6,
        [Display(ResourceType = typeof(Textos), Name = "EgresoSinFleteFazones_Trasmision")]
        EgresoSinFleteFazones = 7,
        [Display(ResourceType = typeof(Textos), Name = "FletesDobleTramo")]
        FletesDobleTramo = 8,
        [Display(ResourceType = typeof(Textos), Name = "CartaPorteRegistro")]
        CartaPorteTransporteAutomotorRegistro = 9,
        [Display(ResourceType = typeof(Textos), Name = "CartaPorteFerroviarioRegistro")]
        CartaPorteVagonFerroviarioRegistro = 10,
        [Display(ResourceType = typeof(Textos), Name = "MuestreoPesajeTransporteAutomotor")]
        MuestreoPesajeTransporteAutomotorRegistro = 11,
        [Display(ResourceType = typeof(Textos), Name = "MuestreoPesajeVagonFerroviario")]
        MuestreoPesajeVagonFerroviarioRegistro = 12,
        [Display(ResourceType = typeof(Textos), Name = "InformarCupo")]
        InformarCupo = 13,
        [Display(ResourceType = typeof(Textos), Name = "ActServicioSapIngresosBodega")]
        IngresosBodega = 14,
        [Display(ResourceType = typeof(Textos), Name = "ActServicioSapZE7550")]
        ZE7550 = 15
    }
}
