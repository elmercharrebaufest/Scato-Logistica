using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{

    public enum Actividades : int
    {
        //Actividades
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarDescuentosEntregador")]
        AutorizarDescuentosEntregador = 100,
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarTransportistaInhabilitado")]
        AutorizarTransportistaInhabilitado = 101,
        [Display(ResourceType = typeof(Textos), Name = "ActBalanzaACero")]
        BalanzaACero = 102,
        [Display(ResourceType = typeof(Textos), Name = "ActCalado")]
        Calado = 103,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarPrecintos")]
        CargarPrecintos = 104,
        [Display(ResourceType = typeof(Textos), Name = "ActControlDePesoEsperado")]
        ControlDePesoEsperado = 105,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoMaximo")]
        ControlPesoMaximo = 106,
        [Display(ResourceType = typeof(Textos), Name = "ActCoordinacion")]
        Coordinacion = 107,
        [Display(ResourceType = typeof(Textos), Name = "ActDocumentoDeIngreso")]
        DocumentoDeIngreso = 108,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarOrdenDeCargaInterna")]
        IngresarOrdenCargaInterna = 109,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeObservaciones")]
        IngresoDeObservaciones = 110,
        [Display(ResourceType = typeof(Textos), Name = "ActListaDeCamiones")]
        ListaDeCamiones = 111,
        [Display(ResourceType = typeof(Textos), Name = "ActPesada")]
        Pesada = 112,
        [Display(ResourceType = typeof(Textos), Name = "ActSalidaDeCentro")]
        SalidaDeCentro = 113,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificacionCamionRechazado")]
        VerificacionCamionRechazado = 114,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarCartaPorte")]
        CargarCartaPorte = 115,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeTransportista")]
        IngresoDeTransportista = 116,
        [Display(ResourceType = typeof(Textos), Name = "ActBajaCTG")]
        BajaCTG = 117,
        [Display(ResourceType = typeof(Textos), Name = "ActConfirmacionDeCargaDescarga")]
        ConfirmacionDeCargaDescarga = 118,
        [Display(ResourceType = typeof(Textos), Name = "ActAnalisisDeCalidad")]
        AnalisisDeCalidad = 119,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoOrigen")]
        ControlPesoOrigen = 120,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarCartaPorteRedespacho")]
        IngresarCartaPorteRedespacho = 121,
        [Display(ResourceType = typeof(Textos), Name = "ActAltaCTG")]
        AltaCTG = 122,
        [Display(ResourceType = typeof(Textos), Name = "ActMovimientoStockSap")]
        MovimientoStockSap = 123,
        [Display(ResourceType = typeof(Textos), Name = "ActExistePedidoDeTraslado")]
        ExistePedidoDeTraslado = 124,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresosPorCompraDeGranos")]
        IngresosPorCompraDeGranos = 125,
        [Display(ResourceType = typeof(Textos), Name = "ActLlegadaADestinoEnRedespacho")]
        LlegadaADestinoEnRedespacho = 126,
        [Display(ResourceType = typeof(Textos), Name = "ActSalidaDeOrigenEnRedespacho")]
        SalidaDeOrigenEnRedespacho = 127,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoNeto")]
        ControlPesoNeto = 128,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarOrdenEntrePlantas")]
        CargarOrdenEntrePlantas = 129,
        [Display(ResourceType = typeof(Textos), Name = "ActIndianapolis")]
        Indianapolis = 157,
        [Display(ResourceType = typeof(Textos), Name = "ActEnEsperaAduana")]
        EnEsperaAduana = 158,
    }
}
