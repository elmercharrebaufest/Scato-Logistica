using System.CodeDom;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Seguridad
{
    /// <summary>
    ///     Este Enum contiene todos los permisos usados en Scato. Estos permisos también deben estar agregados a la base de datos para poder ser mapeados a los roles.
    /// </summary>
    public enum PermisosScato
    {
        //ABMs
        [Display(ResourceType = typeof(Textos), Name = "AbmTipoDocumentoIdentidad")]
        AbmTipoDocumentoIdentidad = 1,
        [Display(ResourceType = typeof(Textos), Name = "AbmMaterial")]
        AbmMaterial = 2,
        [Display(ResourceType = typeof(Textos), Name = "AbmChofer")]
        AbmChofer = 3,
        [Display(ResourceType = typeof(Textos), Name = "AbmAlmacen")]
        AbmAlmacen = 4,
        [Display(ResourceType = typeof(Textos), Name = "AbmTransportista")]
        AbmTransportista = 5,
        [Display(ResourceType = typeof(Textos), Name = "AbmExcepcionesAlControl")]
        AbmExcepcionAlControl = 6,
        [Display(ResourceType = typeof(Textos), Name = "AbmInhabilitacionChofer")]
        AbmInhabilitacionChofer = 7,
        [Display(ResourceType = typeof(Textos), Name = "AbmInhabilitacionCamion")]
        AbmInhabilitacionCamion = 8,
        [Display(ResourceType = typeof(Textos), Name = "AbmCentro")]
        AbmCentro = 9,
        [Display(ResourceType = typeof(Textos), Name = "AbmTipoComercial")]
        AbmTipoComercial = 10,
        [Display(ResourceType = typeof(Textos), Name = "AbmTipoComercialPorWf")]
        AbmTipoComercialPorWf = 11,
        [Display(ResourceType = typeof(Textos), Name = "AbmBalanza")]
        AbmBalanza = 12,
        [Display(ResourceType = typeof(Textos), Name = "AbmCaracteristicaDeCalidad")]
        AbmCaracteristicaDeCalidad = 13,
        [Display(ResourceType = typeof(Textos), Name = "AbmCamara")]
        AbmCamara = 14,
        [Display(ResourceType = typeof(Textos), Name = "AbmMotivo")]
        AbmMotivo = 15,
        [Display(ResourceType = typeof(Textos), Name = "AbmBocaDestino")]
        AbmBocaDestino = 16,
        [Display(ResourceType = typeof(Textos), Name = "AbmHumedimetro")]
        AbmHumedimetro = 17,
        [Display(ResourceType = typeof(Textos), Name = "AbmRol")]
        AbmRol = 18,
        [Display(ResourceType = typeof(Textos), Name = "AbmPermiso")]
        AbmPermiso = 19,
        [Display(ResourceType = typeof(Textos), Name = "AbmUsuario")]
        AbmUsuario = 20,
        [Display(ResourceType = typeof(Textos), Name = "AbmSuplencia")]
        AbmSuplencia = 21,
        [Display(ResourceType = typeof(Textos), Name = "AbmExcepcionEnvioCamara")]
        AbmExcepcionEnvioCamara = 22,
        [Display(ResourceType = typeof(Textos), Name = "AbmEntregador")]
        AbmEntregador = 23,
        [Display(ResourceType = typeof(Textos), Name = "AbmTransaccionSAP")]
        AbmTransaccionSAP = 24,
        [Display(ResourceType = typeof(Textos), Name = "AbmMaterialPorWorkflow")]
        AbmMaterialPorWorkflow = 25,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransSAP")]
        PanelDeControlTransaccionesSap = 26,
        [Display(ResourceType = typeof(Textos), Name = "AbmTaraRomaneo")]
        AbmTaraRomaneo = 27,
        [Display(ResourceType = typeof(Textos), Name = "AbmTalonario")]
        AbmTalonario = 28,
        [Display(ResourceType = typeof(Textos), Name = "AbmModificarDocumentoDeIngreso")]
        AbmModificarDocumentoDeIngreso = 29,
        [Display(ResourceType = typeof(Textos), Name = "AbmReimpresionDeDocumentos")]
        AbmReimpresionDeDocumentos = 30,
        [Display(ResourceType = typeof(Textos), Name = "GenerarArchivosOncca")]
        GenerarArchivosOncca = 31,
        [Display(ResourceType = typeof(Textos), Name = "AbmProveedor")]
        AbmProveedor = 32,
        [Display(ResourceType = typeof(Textos), Name = "AbmImpresiones")]
        AbmImpresiones = 33,
        [Display(ResourceType = typeof(Textos), Name = "AbmFormatoDeImpresion")]
        AbmFormatoDeImpresion = 34,
        [Display(ResourceType = typeof(Textos), Name = "AbmLibroMovimientosExistenciaGranos")]
        AbmLibroMovimientosExistenciaGranos = 35,
        [Display(ResourceType = typeof(Textos), Name = "AbmBorradoDeDocumento")]
        AbmBorradoDeDocumento = 36,
        [Display(ResourceType = typeof(Textos), Name = "AbmTaraContenedor")]
        AbmTaraContenedor = 37,
        [Display(ResourceType = typeof(Textos), Name = "AbmCalle")]
        AbmCalle = 38,
        [Display(ResourceType = typeof(Textos), Name = "AbmCalidadMaterial")]
        AbmCalidadMaterial = 39,
        [Display(ResourceType = typeof(Textos), Name = "AbmCasillero")]
        AbmCasillero = 40,
        [Display(ResourceType = typeof(Textos), Name = "AbmTarjetaBloqueada")]
        AbmTarjetaBloqueada = 41,
        [Display(ResourceType = typeof(Textos), Name = "AbmReciboMunicipal")]
        AbmReciboMunicipal = 42,
        [Display(ResourceType = typeof(Textos), Name = "AbmTarjetaRango")]
        AbmTarjetaRango = 43,
        [Display(ResourceType = typeof(Textos), Name = "AbmTarjetaSupervisor")]
        AbmTarjetaSupervisor = 44,
        [Display(ResourceType = typeof(Textos), Name = "AbmHidraulica")]
        AbmHidraulica = 45,
        [Display(ResourceType = typeof(Textos), Name = "AbmControlDeTiempo")]
        AbmControlDeTiempo = 46,
        [Display(ResourceType = typeof(Textos), Name = "AbmPuestoDeTrabajo")]
        AbmPuestoDeTrabajo = 47,
        [Display(ResourceType = typeof(Textos), Name = "AbmActividadPorDispositivo")]
        AbmActividadPorDispositivo = 48,
        [Display(ResourceType = typeof(Textos), Name = "AbmActividadPorDispositivo")]
        AbmActividadPorBarreraSemaforo = 49,
        [Display(ResourceType = typeof(Textos), Name = "IngresarMotivoQuiebreBarrera")]
        MotivoQuiebreBarrera = 50,
        [Display(ResourceType = typeof(Textos), Name = "EditordeWorkflow")]
        EditordeWorkflow = 51,
        [Display(ResourceType = typeof(Textos), Name = "AbmConsultarDocumentoDeIngreso")]
        AbmConsultarDocumentoDeIngreso = 52,
        [Display(ResourceType = typeof(Textos), Name = "AbmListaDeTareasAutomatizada")]
        AbmListaDeTareasAutomatizada = 53,
        [Display(ResourceType = typeof(Textos), Name = "AbmModalidadPuestoDeTrabajo")]
        AbmModalidadPuestoDeTrabajo = 54,
        [Display(ResourceType = typeof(Textos), Name = "ReimpresionDocumentosImprimirDocumento")]
        ReimpresionDocumentosImprimirDocumento = 55,
        [Display(ResourceType = typeof(Textos), Name = "ReimpresionDocumentosBorrarDocumento")]
        ReimpresionDocumentosBorrarDocumento = 56,
        [Display(ResourceType = typeof(Textos), Name = "LiberacionDeCasilleros")]
        LiberacionDeCasilleros = 57,
        [Display(ResourceType = typeof(Textos), Name = "ConsultaCasillero")]
        ConsultaCasillero = 58,
        [Display(ResourceType = typeof(Textos), Name = "AbmImpresion")]
        AbmImpresora = 59,
        [Display(ResourceType = typeof(Textos), Name = "ExportacionDeArchivos")]
        ExportacionDeArchivos = 60,
        [Display(ResourceType = typeof(Textos), Name = "AbmCiuAnulado")]
        AbmCiuAnulado = 61,
        [Display(ResourceType = typeof(Textos), Name = "AbmVinedoPropio")]
        AbmVinedoPropio = 62,
        [Display(ResourceType = typeof(Textos), Name = "AbmVariedadPorVinedo")]
        AbmVariedadPorVinedo = 63,
        [Display(ResourceType = typeof(Textos), Name = "AbmVinedoTerceros")]
        AbmVinedoTerceros = 64,
        [Display(ResourceType = typeof(Textos), Name = "AbmAsignacionDeRecorrido")]
        AbmAsignacionDeRecorrido = 65,
        [Display(ResourceType = typeof(Textos), Name = "ExportacionDeArchivosINV")]
        ExportacionDeArchivosINV = 66,
        [Display(ResourceType = typeof(Textos), Name = "AbmActividadConCargaAutomatica")]
        AbmActividadConCargaAutomatica = 67,
        [Display(ResourceType = typeof(Textos), Name = "AbmEstablecimiento")]
        AbmEstablecimiento = 68,
        [Display(ResourceType = typeof(Textos), Name = "AbmConsultarDocumentoDeIngresoPorTarjeta")]
        AbmConsultarDocumentoDeIngresoPorTarjeta = 69,
        [Display(ResourceType = typeof(Textos), Name = "AbmEmpresa")]
        AbmEmpresa = 70,
        [Display(ResourceType = typeof(Textos), Name = "AbmTecnologia")]
        AbmTecnologia = 71,
        [Display(ResourceType = typeof(Textos), Name = "AbmProveedorExcluidoIntacta")]
        AbmProveedorExcluidoIntacta = 72,
        [Display(ResourceType = typeof(Textos), Name = "AbmTransmisionASap")]
        AbmTransmisionASap = 73,
        [Display(ResourceType = typeof(Textos), Name = "AbmImpresionPorCentro")]
        AbmImpresionPorCentro = 74,
        [Display(ResourceType = typeof(Textos), Name = "BorrarImpresionPorCentro")]
        BorrarImpresionPorCentro = 75,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlTransaccionesSapCupo")]
        PanelDeControlTransaccionesSapCupo = 76,
        [Display(ResourceType = typeof(Textos), Name = "MovimientoDeBines")]
        MovimientoDeBines = 77,
        [Display(ResourceType = typeof(Textos), Name = "AbmFirma")]
        AbmFirma = 78,
        [Display(ResourceType = typeof(Textos), Name = "AbmExcepcionAlDescuento")]
        AbmExcepcionAlDescuento = 79,
        [Display(ResourceType = typeof(Textos), Name = "AbmAdministracionDistancias")]
        AbmAdministracionDistancias = 80,
        [Display(ResourceType = typeof(Textos), Name = "BorrarDocumentosTerminados")]
        BorrarDocumentosTerminados = 81,
        [Display(ResourceType = typeof(Textos), Name = "BorrarDocumentosNoTerminados")]
        BorrarDocumentosNoTerminados = 82,
        [Display(ResourceType = typeof(Textos), Name = "AbmTipoDeVehiculo")]
        AbmTipoDeVehiculo = 83,
        [Display(ResourceType = typeof(Textos), Name = "AbmStockDeEstablecimiento")]
        AbmStockDeEstablecimiento = 84,
        [Display(ResourceType = typeof(Textos), Name = "AbmCosecha")]
        AbmCosecha = 85,
        [Display(ResourceType = typeof(Textos), Name = "AbmCategoria")]
        AbmCategoria = 86,
        [Display(ResourceType = typeof(Textos), Name = "AbmMatricula")]
        AbmMatricula = 87,
        [Display(ResourceType = typeof(Textos), Name = "Entregador_EnvioCamaraDirecto")]
        EnvioCamaraDirecto = 88,
        [Display(ResourceType = typeof(Textos), Name = "AbmAvisoDeQuiebre")]
        AbmAvisoDeQuiebre = 89,
        [Display(ResourceType = typeof(Textos), Name = "AbmNirs")]
        AbmNirs = 90,
        [Display(ResourceType = typeof(Textos), Name = "HistoricoDeCamiones")]
        HistoricoDeCamiones = 91,
        [Display(ResourceType = typeof(Textos), Name = "AbmEstacionMeteorologica")]
        AbmEstacionMeteorologica = 92,
        [Display(ResourceType = typeof(Textos), Name = "HabilitacionDeVehiculos_Titulo")]
        HabilitacionDeVehiculos = 93,
        [Display(ResourceType = typeof(Textos), Name = "AbmAnalisisObligatorio_Titulo")]
        AbmAnalisisObligatorio = 94,
        [Display(ResourceType = typeof(Textos), Name = "AbmReglasDeAnalisisObligatorio_Titulo")]
        AbmReglasDeAnalisisObligatorio = 95,
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeComando_CaladoEnPlanta")]
        PuestoDeComando_CaladoEnPlanta = 96,
        [Display(ResourceType = typeof(Textos), Name = "AbmCliente")]
        AbmCliente = 97,
        [Display(ResourceType = typeof(Textos), Name = "TransmisionASapManual")]
        TransmisionASapManual = 98,
        [Display(ResourceType = typeof(Textos), Name = "DocumentoExterno")]
        DocumentoExterno = 99,
        [Display(ResourceType = typeof(Textos), Name = "AbmCategoriaCamiones")]
        AbmCategoriaCamiones = 604,

        //Permisos Actividades
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarDescuentosEntregador")]
        ActividadAutorizarDescuentosEntregador = 100,
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarTransportistaInhabilitado")]
        ActividadAutorizarTransportistaInhabilitado = 101,
        [Display(ResourceType = typeof(Textos), Name = "ActBalanzaACero")]
        ActividadBalanzaACero = 102,
        [Display(ResourceType = typeof(Textos), Name = "ActCalado")]
        ActividadCalado = 103,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarPrecintos")]
        ActividadCargarPrecintos = 104,
        [Display(ResourceType = typeof(Textos), Name = "ActControlDePesoEsperado")]
        ActividadControlDePesoEsperado = 105,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoMaximo")]
        ActividadControlPesoMaximo = 106,
        [Display(ResourceType = typeof(Textos), Name = "ActCoordinacion")]
        ActividadCoordinacion = 107,
        [Display(ResourceType = typeof(Textos), Name = "ActDocumentoDeIngreso")]
        ActividadDocumentoDeIngreso = 108,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarOrdenDeCargaInterna")]
        ActividadIngresarOrdenCargaInterna = 109,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeObservaciones")]
        ActividadIngresoDeObservaciones = 110,
        [Display(ResourceType = typeof(Textos), Name = "ActListaDeCamiones")]
        ActividadListaDeCamiones = 111,
        [Display(ResourceType = typeof(Textos), Name = "ActPesada")]
        ActividadPesada = 112,
        [Display(ResourceType = typeof(Textos), Name = "ActSalidaDeCentro")]
        ActividadSalidaDeCentro = 113,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificacionCamionRechazado")]
        ActividadVerificacionCamionRechazado = 114,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarCartaPorte")]
        ActividadCargarCartaPorte = 115,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeTransportista")]
        ActividadIngresoDeTransportista = 116,
        [Display(ResourceType = typeof(Textos), Name = "ActBajaCTG")]
        ActividadBajaCTG = 117,
        [Display(ResourceType = typeof(Textos), Name = "ActConfirmacionDeCargaDescarga")]
        ActividadConfirmacionDeCargaDescarga = 118,
        [Display(ResourceType = typeof(Textos), Name = "ActAnalisisDeCalidad")]
        ActividadAnalisisDeCalidad = 119,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoOrigen")]
        ActividadControlPesoOrigen = 120,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarCartaPorteRedespacho")]
        ActividadIngresarCartaPorteRedespacho = 121,
        [Display(ResourceType = typeof(Textos), Name = "ActAltaCTG")]
        ActividadAltaCTG = 122,
        [Display(ResourceType = typeof(Textos), Name = "ActMovimientoStockSap")]
        ActividadMovimientoStockSap = 123,
        [Display(ResourceType = typeof(Textos), Name = "ActExistePedidoDeTraslado")]
        ActividadExistePedidoDeTraslado = 124,
        [Display(ResourceType = typeof(Textos), Name = "ActSalidaDeOrigenEnRedespacho")]
        ActividadSalidaDeOrigenEnRedespachos = 125,
        [Display(ResourceType = typeof(Textos), Name = "ActLlegadaADestinoEnRedespacho")]
        ActividadLlegadaADestinoEnRedespachos = 126,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresosPorCompraDeGranos")]
        ActividadIngresosPorCompraDeGranos = 127,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoNeto")]
        ActividadControlPesoNeto = 128,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarOrdenDeDescarga")]
        ActividadCargarOrdenDeDescarga = 129,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarOrdenDeCargaFas")]
        ActividadIngresarOrdenDeCargaFas = 130,
        [Display(ResourceType = typeof(Textos), Name = "ActRomaneo")]
        ActividadRomaneo = 131,
        [Display(ResourceType = typeof(Textos), Name = "ActVisteo")]
        ActividadVisteo = 132,
        [Display(ResourceType = typeof(Textos), Name = "ActEgresosMaterialNoProductivo")]
        ActividadEgresosMaterialNoProductivo = 133,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeLote")]
        ActividadIngresoDeLote = 134,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificacionCot")]
        VerificacionCot = 135,
        [Display(ResourceType = typeof(Textos), Name = "ActValidarContrato")]
        ActividadValidarContrato = 136,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarCartaPorteRedespacho")]
        ActividadIngresarCartaPorteRedespachoPrestamo = 137,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarCartaPorte")]
        ActividadCargarCartaPortePrestamo = 138,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarOrdenDeDescargaFason")]
        ActividadCargarOrdenDeDescargaFason = 139,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarOrdenDeCargaInternaFason")]
        ActividadIngresarOrdenCargaInternaFason = 140,
        [Display(ResourceType = typeof(Textos), Name = "ActBajaCTGDefinitivo")]
        ActividadBajaCTGDefinitivo = 141,
        [Display(ResourceType = typeof(Textos), Name = "ActControlDeIngreso")]
        ActividadControlDeIngreso = 142,
        [Display(ResourceType = typeof(Textos), Name = "ActServicioSapIngresosEgresosFazones")]
        ActividadIngresosEgresosFazones = 143,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificacionSalidaFlete")]
        ActividadVerificacioneSalidaFlete = 144,
        [Display(ResourceType = typeof(Textos), Name = "ActPesaNeto")]
        ActividadPesaNeto = 145,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarCartaPorteRedespachoDesvio")]
        ActividadCargarCartaPorteRedespachoDesvio = 146,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoNumeroCot")]
        ActividadIngresoNumeroCot = 147,
        [Display(ResourceType = typeof(Textos), Name = "ActFletesDobleTramo")]
        ActividadFletesDobleTramo = 148,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarOrdenEntrePlantas")]
        ActividadCargarOrdenEntrePlantas = 149,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoRemito")]
        ActividadIngresoRemito = 150,
        [Display(ResourceType = typeof(Textos), Name = "ActDescargaUnidad")]
        ActividadDescargaUnidad = 151,
        [Display(ResourceType = typeof(Textos), Name = "ActControlPesoNetoDescargaUnidad")]
        ActividadControlPesoNetoDescargaUnidad = 152,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarOrdenDeCargaContenedor")]
        ActividadCargarOrdenDeCargaContenedor = 153,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoRemitoTerceros")]
        ActividadIngresoRemitoTerceros = 154,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarCartaPorteFason")]
        ActividadCargarCartaPorteFason = 155,
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarTiempoEnTransito")]
        ActividadAutorizarTiempoEnTransito = 156,
        [Display(ResourceType = typeof(Textos), Name = "ActEmitirReciboMunicipal")]
        ActividadEmitirReciboMunicipal = 157,
        [Display(ResourceType = typeof(Textos), Name = "ActPuestoComando")]
        ActividadPuestoComando = 158,
        [Display(ResourceType = typeof(Textos), Name = "ActControlDeBalanza")]
        ActividadControlDeBalanza = 159,
        [Display(ResourceType = typeof(Textos), Name = "ActIndianapolis")]
        ActividadEnEsperaIndianapolis = 160,
        [Display(ResourceType = typeof(Textos), Name = "ActEnEsperaAduana")]
        ActividadEnEsperaAduana = 161,
        [Display(ResourceType = typeof(Textos), Name = "ActPesadaBruto")]
        ActividadPesadaBruto = 162,
        [Display(ResourceType = typeof(Textos), Name = "ActPesadaTara")]
        ActividadPesadaTara = 163,
        [Display(ResourceType = typeof(Textos), Name = "ActEnPlayaExterna")]
        ActividadEnPlayaExterna = 164,
        [Display(ResourceType = typeof(Textos), Name = "ActEnTransito")]
        ActividadEnTransito = 165,
        [Display(ResourceType = typeof(Textos), Name = "ActEgresoSinFleteFasones")]
        ActividadEgresoSinFleteFasones = 166,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoCIU")]
        ActividadIngresoCIU = 167,
        [Display(ResourceType = typeof(Textos), Name = "ActRemitoBodegaUvaPropia")]
        ActividadRemitoBodegaUvaPropia = 168,
        [Display(ResourceType = typeof(Textos), Name = "ActAutorizarRecepcionUvas")]
        ActividadAutorizarRecepcionUvas = 169,
        [Display(ResourceType = typeof(Textos), Name = "ActDistribucionDeAlmacenes")]
        ActividadDistribucionDeAlmacenes = 170,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoBinSalida")]
        ActividadIngresoBinSalida = 171,
        [Display(ResourceType = typeof(Textos), Name = "ActRemitoBodegaUvaTerceros")]
        ActividadRemitoBodegaUvaTerceros = 172,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarHojaDeRuta")]
        ActividadCargarHojaDeRuta = 173,
        [Display(ResourceType = typeof(Textos), Name = "ActCargarHojaDeRutaYerbatera")]
        ActividadCargarHojaDeRutaYerbatera = 174,
        [Display(ResourceType = typeof(Textos), Name = "ActAsignacionDeEstablecimiento")]
        ActividadAsignacionDeEstablecimiento = 175,
        [Display(ResourceType = typeof(Textos), Name = "ActPesadaBrutoVagon")]
        ActividadPesadaBrutoVagon = 176,
        [Display(ResourceType = typeof(Textos), Name = "ActPesadaTaraVagon")]
        ActividadPesadaTaraVagon = 177,
        [Display(ResourceType = typeof(Textos), Name = "ActRemitoBodegaVino")]
        ActividadRemitoBodegaVino = 178,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificarImpresion")]
        ActividadVerificarImpresion = 179,
        [Display(ResourceType = typeof(Textos), Name = "ActServicioSapIngresosBodega")]
        ActividadIngresosBodega = 180,
        [Display(ResourceType = typeof(Textos), Name = "Avanzar")]
        ForzarCereo = 181,
        [Display(ResourceType = typeof(Textos), Name = "GenerarArchivoDeMovimientos_Titulo")]
        GenerarArchivosDeMovimientos = 182,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresoDeDatosDeExportacion")]
        IngresoDeDatosDeExportacion = 183,
        [Display(ResourceType = typeof(Textos), Name = "CargaDeCupo")]
        CargaDeCupo = 184,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarCartaPorteRedespachoImportaciones")]
        ActividadIngresarCartaPorteRedespachoImportaciones = 185,
        [Display(ResourceType = typeof(Textos), Name = "ActServicioSapZE7550")]
        ActividadServicioSapZE7550 = 186,
        [Display(ResourceType = typeof(Textos), Name = "ActPasoPorBalanza")]
        PasoPorBalanza = 187,
        [Display(ResourceType = typeof(Textos), Name = "ActPesadaCargaExportacion")]
        ActividadPesadaCargaExportacion = 188,
        [Display(ResourceType = typeof(Textos), Name = "ActIngresarCartaPorteRedespachoMercaderia")]
        ActividadIngresarCartaPorteRedespachoMercaderia = 189,
        [Display(ResourceType = typeof(Textos), Name = "ActVerificarLimiteDeCreditoVentaEnSAP")]
        ActividadVerificarLimiteDeCreditoVentaEnSAP = 190,
        [Display(ResourceType = typeof(Textos), Name = "ActCaladoEnPlanta")]
        ActividadCaladoEnPlanta = 191,
        [Display(ResourceType = typeof(Textos), Name = "CancelarPesadaCargaExportacion")]
        CancelarPesadaCargaExportacion = 192,
        [Display(ResourceType = typeof(Textos), Name = "ActCaladoRechazar")]
        ActividadCaladoRechazar = 193,
        [Display(ResourceType = typeof(Textos), Name = "CobroImpuestoMunicipal_Titulo")]
        PagoDeReciboMunicipal = 194,
        [Display(ResourceType = typeof(Textos), Name = "DevolucionImpuestoMunicipal_Titulo")]
        DevolucionDeReciboMunicipal = 195,
        [Display(ResourceType = typeof(Textos), Name = "BalanzaAutomatica")]
        BalanzaAutomatica = 196,
        [Display(ResourceType = typeof(Textos), Name = "ActSalidaDeCentroPlaya")]
        ActividadSalidaDeCentroPlaya = 197,
        [Display(ResourceType = typeof(Textos), Name = "CamionDemorado")]
        CamionDemorado = 198,
        [Display(ResourceType = typeof(Textos), Name = "ActEnEsperaHB4")]
        EnEsperaHB4= 199,
        [Display(ResourceType = typeof(Textos), Name = "ActEsperaConfirmacion")]
        ActividadEsperaConfirmacion = 700,
        
        //Permisos Generales
        [Display(ResourceType = typeof(Textos), Name = "IniciarWorkflow")]
        IniciarWorkflow = 200,
        [Display(ResourceType = typeof(Textos), Name = "ArmarLote")]
        ArmarLote = 201,
        [Display(ResourceType = typeof(Textos), Name = "BuscarLote")]
        BuscarLote = 202,
        [Display(ResourceType = typeof(Textos), Name = "TablaConversionMaterial")]
        TablaConversionMaterial = 203,
        [Display(ResourceType = typeof(Textos), Name = "TablaConversionProcedencia")]
        TablaConversionProcedencia = 204,
        [Display(ResourceType = typeof(Textos), Name = "TablaConversionGrupo")]
        TablaConversionGrupo = 205,
        [Display(ResourceType = typeof(Textos), Name = "TablaConversionCaracteristica")]
        TablaConversionCaracteristica = 206,
        [Display(ResourceType = typeof(Textos), Name = "AjustarCalidad_Titulo")]
        AjustarCalidad = 207,
        [Display(ResourceType = typeof(Textos), Name = "Micromuestra_Titulo")]
        GeneracionMicromuestras = 208,
        [Display(ResourceType = typeof(Textos), Name = "ModificaImpresiones")]
        ModificaImpresiones = 209,
        [Display(ResourceType = typeof(Textos), Name = "AjusteDeStock_Titulo")]
        AjusteDeStock = 210,
        [Display(ResourceType = typeof(Textos), Name = "ActAsignacionTarjetaDeAcceso")]
        ActividadAsignacionTarjetaDeAcceso = 211,
        [Display(ResourceType = typeof(Textos), Name = "ReasignacionDeTarjetas_Titulo")]
        ReasignacionDeTarjetas = 212,
        [Display(ResourceType = typeof(Textos), Name = "ImpresionTarjetaDeAcceso_Titulo")]
        ImpresionTarjetaDeAcceso = 213,
        [Display(ResourceType = typeof(Textos), Name = "EtiquetaAuditoria_Titulo")]
        EtiquetaAuditoria = 214,
        [Display(ResourceType = typeof(Textos), Name = "CotManual")]
        IngresarCotManual = 215,
        [Display(ResourceType = typeof(Textos), Name = "EjecucionManual")]
        EjecucionManual = 216,
        [Display(ResourceType = typeof(Textos), Name = "ArmarLoteBiotecnologia")]
        ArmarLoteBiotecnologia = 217,
        [Display(ResourceType = typeof(Textos), Name = "AjusteYStockBines_Titulo")]
        AjusteYStockBines = 218,
        [Display(ResourceType = typeof(Textos), Name = "TablaConversionCentro")]
        TablaConversionCentro = 219,
        [Display(ResourceType = typeof(Textos), Name = "AsignacionTicketMunicipal")]
        AsignacionTicketMunicipal = 220,
        [Display(ResourceType = typeof(Textos), Name = "PanelServerAppPool")]
        PanelServerAppPool = 221,
        [Display(ResourceType = typeof(Textos), Name = "PanelWorkflows")]
        PanelWorkflows = 222,
        [Display(ResourceType = typeof(Textos), Name = "EliminarWorkflows")]
        EliminarWorkflows = 223,
        [Display(ResourceType = typeof(Textos), Name = "WorkflowMovimientoDeBines")]
        WorkflowMovimientoDeBines = 224,
        [Display(ResourceType = typeof(Textos), Name = "GraficoDePlanta_Titulo")]
        GraficoDePlanta = 225,
        [Display(ResourceType = typeof(Textos), Name = "PanelBajaCtgDefinitiva")]
        PanelBajaCtgDefinitiva = 226,
        [Display(ResourceType = typeof(Textos), Name = "WebMobile")]
        WebMobile = 227,
        [Display(ResourceType = typeof(Textos), Name = "CamionesRechazados")]
        CamionesRechazados = 228,
        [Display(ResourceType = typeof(Textos), Name = "AsignacionDeContingencia_Titulo")]
        AsignacionDeContingencia = 229,
        [Display(ResourceType = typeof(Textos), Name = "RangosDeRedondeo")]
        RangosDeRedondeo = 230,
        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto")]
        OperacionesPuerto = 231,
        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuertoCrearCarga")]
        OperacionesPuertoCrearCarga = 232,
        [Display(ResourceType = typeof(Textos), Name = "ControlDeCalado")]
        ControlDeCalado = 233,
        [Display(ResourceType = typeof(Textos), Name = "PinchazosPorCalada")]
        PinchazosPorCalada = 262,
        [Display(ResourceType = typeof(Textos), Name = "PanelDeControlUnrenport_Titulo")]
        PanelDeControlUnrenport = 234,
        [Display(ResourceType = typeof(Textos), Name = "ActualizarFotoCartaPorte")]
        ActualizarFotoCartaPorte = 235,
        [Display(ResourceType = typeof(Textos), Name = "CamionesPendientesMesa")]
        CamionesPendientesMesa = 236,
        [Display(ResourceType = typeof(Textos), Name = "CamionesPendientesNoGranos")]
        CamionesPendientesNoGranos = 258,

        [Display(ResourceType = typeof(Textos), Name = "DestinoPuerto")]
        DestinoPuerto = 237,
        [Display(ResourceType = typeof(Textos), Name = "BodegaPuerto")]
        BodegaPuerto = 238,
        [Display(ResourceType = typeof(Textos), Name = "ExportadorPuerto")]
        ExportadorPuerto = 239,
        [Display(ResourceType = typeof(Textos), Name = "MaterialPuertoPuerto")]
        MaterialPuertoPuerto = 240,
        [Display(ResourceType = typeof(Textos), Name = "EmbarqueLiquidoPuerto")]
        EmbarqueLiquidoPuerto = 241,
        [Display(ResourceType = typeof(Textos), Name = "VaporPuerto")]
        VaporPuerto = 242,
        [Display(ResourceType = typeof(Textos), Name = "BalanzaPuerto")]
        BalanzaPuerto = 243,
        [Display(ResourceType = typeof(Textos), Name = "ScatoPuerto")]
        ScatoPuerto = 244,
        [Display(ResourceType = typeof(Textos), Name = "LoteAuditoria")]
        ArmarLoteAuditoria = 245,
        [Display(ResourceType = typeof(Textos), Name = "ModificarDatosExportacion")]
        ModificarDatosExportacion = 246,
        [Display(ResourceType = typeof(Textos), Name = "VisualizarVideoCamaras")]
        VisualizarVideoCamaras = 247,
        [Display(ResourceType = typeof(Textos), Name = "EmbarquesPorBuques")]
        EmbarquesPorBuques = 248,

        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCalle")]
        EstadoDeCalle = 249,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCalleLlamar")]
        EstadoDeCalleLlamar = 250,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCalleCancelar")]
        EstadoDeCalleCancelar = 251,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCallePlayero")]
        EstadoDeCallePlayero = 252,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCalleCalador")]
        EstadoDeCalleCalador = 253,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCalleMoverRechazado")]
        EstadoDeCalleMoverRechazado = 254,
        [Display(ResourceType = typeof(Textos), Name = "EstadoDeCallePlayaInterna")]
        EstadoDeCallePlayaInterna = 255,
        [Display(ResourceType = typeof(Textos), Name = "VisualizarVideoCamarasBalanza")]
        VisualizarVideoCamarasBalanza = 256,
        [Display(ResourceType = typeof(Textos), Name = "EtiquetaPuerto_Titulo")]
        EtiquetaPuerto = 257,
        [Display(ResourceType = typeof(Textos), Name = "Reasignacion_Calles_PostCalado")]
        ReasignacionCallesPostCalado = 259,
        [Display(ResourceType = typeof(Textos), Name = "Llamado_De_Filas_Automatico")]
        LlamadoDeFilasAutomatico = 260,
        [Display(ResourceType = typeof(Textos), Name = "Cambio_De_Material_En_Filas")]
        CambioDeMaterialEnFilas = 261,
        [Display(ResourceType = typeof(Textos), Name = "VisualizarVideoCamarasExportacion")]
        VisualizarVideoCamarasExportacion = 263,
        [Display(ResourceType = typeof(Textos), Name = "MonitorCPECacheada")]
        MonitorCPECacheada = 264,
        [Display(ResourceType = typeof(Textos), Name = "AbmConfiguracionEficienciaCalado")]
        AbmConfiguracionEficienciaCalado = 266,

        //Permisos Notificaciones
        Balanceros = 300,
        Administradores = 301,
        PuestoComando = 302,
        IngresoPlayaInterna = 303,
        Entregadores = 304,
        [Display(ResourceType = typeof(Textos), Name = "NotificacionAplicacion")]
        NotificacionAplicacion = 305,

         //Permisos Reportes
         [Display(ResourceType = typeof(Textos), Name = "ListadoDeArribosAPlanta")]
        ListadoDeArribosAPlanta = 400,
        [Display(ResourceType = typeof(Textos), Name = "PlanillaF515")]
        PlanillaF515 = 401,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeIngresoDeGranos")]
        ListadoDeIngresoDeGranos = 402,
        [Display(ResourceType = typeof(Textos), Name = "InformeDeGestionActual")]
        InformeDeGestionActual = 403,
        [Display(ResourceType = typeof(Textos), Name = "InformeDeGestionHistorico")]
        InformeDeGestionHistorico = 404,
        [Display(ResourceType = typeof(Textos), Name = "ReporteDeSeguimientoYControl")]
        ReporteDeSeguimientoYControl = 405,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCamionesRechazados")]
        ListadoDeCamionesRechazados = 406,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeDiferenciasDePesoEnRedespacho")]
        ListadoDeDiferenciasDePesoEnRedespacho = 407,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeInconsistencias")]
        ListadoDeInconsistencias = 408,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDePendientesDeArmadoDeLote")]
        ListadoDePendientesDeArmadoDeLote = 409,
        [Display(ResourceType = typeof(Textos), Name = "EmisionDeDetalleDeLote")]
        EmisionDeDetalleDeLote = 410,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeBajasDeCTGManual")]
        ListadoDeBajasDeCTGManual = 411,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCambiosModalidadBalanza")]
        ListadoDeCambiosModalidadBalanza = 412,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeExcepcionesDeControl")]
        ListadoDeExcepcionesDeControl = 413,
        [Display(ResourceType = typeof(Textos), Name = "ListadoPonderadoPorCaracteristica")]
        ListadoPonderadoPorCaracteristica = 414,
        [Display(ResourceType = typeof(Textos), Name = "ConsultaDatosDeVehiculos")]
        ConsultaDatosDeVehiculos = 415,
        [Display(ResourceType = typeof(Textos), Name = "LibroDeMovimientosYExistenciaDeGranos")]
        LibroDeMovimientosYExistenciaDeGranos = 416,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeResumen")]
        ListadoDeResumen = 417,
        [Display(ResourceType = typeof(Textos), Name = "InformedeDescarga")]
        InformedeDescarga = 418,
        [Display(ResourceType = typeof(Textos), Name = "ListadodeRomaneo")]
        ListadodeRomaneo = 419,
        [Display(ResourceType = typeof(Textos), Name = "ListadodeDiferenciadePeso")]
        ListadodeDiferenciadePeso = 420,
        [Display(ResourceType = typeof(Textos), Name = "ListadodeRechazos")]
        ListadodeRechazos = 421,
        [Display(ResourceType = typeof(Textos), Name = "ReporteAnexoINASE")]
        ReporteAnexoINASE = 422,
        [Display(ResourceType = typeof(Textos), Name = "ReporteResumenDeRecepcion")]
        ReporteResumenDeRecepcion = 423,

        [Display(ResourceType = typeof(Textos), Name = "ReporteControlBalanza_Titulo")]
        ReporteControlBalanza = 424,
        [Display(ResourceType = typeof(Textos), Name = "ReporteResumenGeneralPorBodega_Titulo")]
        ReporteResumenGeneralPorBodega = 425,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoDeCiusPorVariedad_Titulo")]
        ReporteListadoDeCiusPorVariedad = 426,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoDeCiusPorViñatero_Titulo")]
        ReporteListadoDeCiusPorViñatero = 427,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoDeCiusPorViñateroyVariedad_Titulo")]
        ReporteListadoDeCiusPorViñedoyVariedad = 428,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoDeRecepcionesDeBodegas_Titulo")]
        ReporteListadoDeRecepcionesDeBodegas = 429,
        [Display(ResourceType = typeof(Textos), Name = "ReporteIngresoDeUvasPorTipoDeOperacionYVariedad_Titulo")]
        ReporteIngresoDeUvasPorTipoDeOperacionYVariedad = 430,
        [Display(ResourceType = typeof(Textos), Name = "ReporteIngresoDeUvasPorVariedad_Titulo")]
        ReporteIngresoDeUvasPorVariedad = 431,
        [Display(ResourceType = typeof(Textos), Name = "ReporteIngresoDeUvasPorViñateroYVariedad_Titulo")]
        ReporteIngresoDeUvasPorViñateroYVariedad = 432,
        [Display(ResourceType = typeof(Textos), Name = "ReporteIngresoDeUvasPorViñatero_Titulo")]
        ReporteIngresoDeUvasPorViñatero = 433,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCalidades")]
        ListadoDeCalidades = 434,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoQuiebreBarrera_Titulo")]
        ReporteListadoQuiebreBarrera = 436,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoDocumentosAnulados_Titulo")]
        ReporteListadoDocumentosAnulados = 437,
        [Display(ResourceType = typeof(Textos), Name = "RegistroUsoTarjetaSupervisor_Titulo")]
        RegistroUsoTarjetaSupervisor = 438,
        [Display(ResourceType = typeof(Textos), Name = "ListadoRecibosMunicipales_Titulo")]
        ListadoRecibosMunicipales = 439,
        [Display(ResourceType = typeof(Textos), Name = "LogDeAjustesOncca_Titulo")]
        LogDeAjustesOncca = 440,
        [Display(ResourceType = typeof(Textos), Name = "ListadoCambioDeModalidadDePuestoDeTrabajo_Titulo")]
        ListadoCambioDeModalidadDePuestoDeTrabajo = 441,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCartaDePorteFason_Titulo")]
        ListadoDeCartaDePorteFason = 442,
        [Display(ResourceType = typeof(Textos), Name = "ReporteMuestrasMonsanto_Titulo")]
        ReporteMuestrasMonsanto = 443,
        [Display(ResourceType = typeof(Textos), Name = "Log_Humedimetro")]
        LogHumedimetro = 444,
        [Display(ResourceType = typeof(Textos), Name = "Reporte_InformacionMonsanto")]
        ReporteInformacionMonsanto = 445,
        [Display(ResourceType = typeof(Textos), Name = "DetalleMuestraAuditoria")]
        DetalleMuestraAuditoria = 446,
        [Display(ResourceType = typeof(Textos), Name = "ExcepcionesAlPagoTicketMunicipal")]
        ExcepcionAlPagoTicketMunicipal = 447,
        [Display(ResourceType = typeof(Textos), Name = "ReporteListadoCuentaCorrienteBins")]
        ReporteListadoCuentaCorrienteBins = 448,
        [Display(ResourceType = typeof(Textos), Name = "ReporteStockDetalladoDeBines")]
        ReporteStockDetalladoDeBines = 449,
        [Display(ResourceType = typeof(Textos), Name = "ReporteDePasoPorCero")]
        ReporteDePasoPorCero = 450,
        [Display(ResourceType = typeof(Textos), Name = "LogDeImpresiones")]
        LogDeImpresiones = 451,
        [Display(ResourceType = typeof(Textos), Name = "ReporteCalidadYMerma")]
        ReporteCalidadYMerma = 452,
        [Display(ResourceType = typeof(Textos), Name = "ListadoLoginUsuarios")]
        ListadoLoginUsuarios = 453,
        [Display(ResourceType = typeof(Textos), Name = "ReporteStockEPA")]
        ReporteStockEPA = 454,
        [Display(ResourceType = typeof(Textos), Name = "ListadoCamionesInhabilitados")]
        ListadoCamionesInhabilitados = 455,
        [Display(ResourceType = typeof(Textos), Name = "ListadoChoferesInhabilitados")]
        ListadoChoferesInhabilitados = 456,
        [Display(ResourceType = typeof(Textos), Name = "InformeDeEficienciaDeHidraulicas")]
        InformeDeEficienciaDeHidraulicas = 457,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeCCPPPorProcedencia")]
        ListadoDeCCPPPorProcedencia = 458,
        [Display(ResourceType = typeof(Textos), Name = "ListadoDeReasignacionDeTarjeta")]
        ListadoDeReasignacionDeTarjeta = 459,
        [Display(ResourceType = typeof(Textos), Name = "DetalleDeMovimientoIngreso")]
        DetalleDeMovimientoIngreso = 460,
        [Display(ResourceType = typeof(Textos), Name = "ReconocimientoDePatentes")]
        ReconocimientoDePatentes = 461,
        [Display(ResourceType = typeof(Textos), Name = "InformeTiemposDePesada")]
        InformeTiemposDePesada = 462,
        [Display(ResourceType = typeof(Textos), Name = "AutorizacionesExcesoDeTiempo")]
        AutorizacionesExcesoDeTiempo = 463,
        [Display(ResourceType = typeof(Textos), Name = "ReporteContingencia")]
        ReporteContingencia = 464,
        [Display(ResourceType = typeof(Textos), Name = "ReporteMuestrasAuditoria")]
        ReporteMuestrasAuditoria = 465,
        [Display(ResourceType = typeof(Textos), Name = "ReporteCpOtrosPuertos")]
        ReporteCpOtrosPuertos = 466,
        [Display(ResourceType = typeof(Textos), Name = "ReporteCpOtrosPuertosSap")]
        ReporteCpOtrosPuertosSap = 467,
        [Display(ResourceType = typeof(Textos), Name = "InformeDeEficienciaDeHidraulicasDiario")]
        InformeDeEficienciaDeHidraulicasDiario = 468,
        [Display(ResourceType = typeof(Textos), Name = "ReporteModalidadCalador")]
        ReporteModalidadCalador = 469,
        [Display(ResourceType = typeof(Textos), Name = "ComparacionCalidad")]
        ComparacionCalidad = 470,
        [Display(ResourceType = typeof(Textos), Name = "ReporteInactividadCalado")]
        ReporteInactividadCalado = 471,

        //Permisos Puerto
        [Display(ResourceType = typeof(Textos), Name = "PreLineUp")]
        PreLineUp = 600,
        [Display(ResourceType = typeof(Textos), Name = "LineUp")]
        LineUp = 601,
        [Display(ResourceType = typeof(Textos), Name = "LineUpLectura")]
        LineUpLectura = 602,
        [Display(ResourceType = typeof(Textos), Name = "LineUpExportar")]
        LineUpExportar = 603,
        [Display(ResourceType = typeof(Textos), Name = "PuestoPausado")]
        PuestoPausado = 605,
        [Display(ResourceType = typeof(Textos), Name = "HidraulicasEspeciales")]
        HidraulicasEspeciales = 606,


        [Display(ResourceType = typeof(Textos), Name = "VerBalanzasPesada")]
        VerBalanzasPesada = 607,

        [Display(ResourceType = typeof(Textos), Name = "VerRevertirRechazoVagones")]
        VerRevertirRechazoVagones = 608,
    }
}
