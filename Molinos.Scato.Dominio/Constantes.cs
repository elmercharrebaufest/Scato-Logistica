using System.Dynamic;

namespace Molinos.Scato.Dominio
{
    public static class Constantes
    {
        public struct ValoresPorDefecto
        {
            public const string CupoGenerico = "MOL1111/11111111";
            public const long CuitMOA = 30715118773;
            public const string NumeroRemitoGenerico = "1111-11111111";

            public const string ColorFondoSojaEPA = "#FC4D75";
            public const string ColorTextoSojaEPA = "#FFFFFF";
            public const string ColorFondoSojaEUDR = "#353DFF";
            public const string ColorTextoSojaEUDR = "#FFFFFF";
            public const string ColorFondoSojaEPAyEUDR = "#BC35FF";
            public const string ColorTextoSojaEPAyEUDR = "#FFFFFF";
            public const string ColorFondoSojaSustentable = "#006302";
            public const string ColorTextoSojaSustentable = "#FFFFFF";

            public const string CodigoSapTPR = "70809606";
            public const string CodigoSapACA = "50012088";
            public const string EstablecimientoACA = "21145";
            public const string ColorTextoDemoradoTasaMunicipal = "#FF2900";
            public const string RazonSocialMOA = "Molinos Agro S.A.";
        }

        public struct IntercomunicadorDireccion
        {
            public const string HaciaLaWeb = "2web";
            public const string DesdeLaWeb = "web2";
        }

        public struct NotificacionGrupos
        {
            public const string Intercomunicador = "Intercomunicador";
            public const string SensoresBarreraHidraulica = "SensoresBarreraHidraulica";
            public const string NotificacionAplicacion = "NotificacionAplicacion";
            public const string EstadoServicioExterno = "EstadoServicioExterno";
        }

        public struct Entidad
        {
            public const string Visteo = "EVIST";
        }

        public struct TipoDeActividad
        {
            public const string Rechazar = "TRECH";
        }

        public struct CodigosEventos
        {
            public const string CambioEstadoIntercomunicador = "CambioEstadoIntercomunicador";
            public const string CambioEstadoSensorCamaraALPR = "CambioEstadoSensorCamaraALPR";
            public const string CambioEstadoSensorGeneral = "CambioEstadoSensorGeneral";
            public const string CambioEstadoSensorCirculacion = "CambioEstadoSensorCirculacion";
            public const string CambioEstadoCamaraALPR = "CambioEstadoCamaraALPR";
            public const string VehiculoDetectado = "VehiculoDetectado";
            public const string IdentificacionVehicular = "IdentificacionVehicular";
        }

        public struct AFIPCodigoDeError
        {
            public const string NoExistenSolicitudes = "800";
            public const string ErrorPDFNoGenerado = "550";
        }

        public struct AsignacionDeEstablecimientoRangos
        {
            public const int Desde = 90000;
            public const int Hasta = 99998;
        }

        public struct TipoDocumentoChofer
        {
            public const string Cuit = "CUI";
            public const string Dni = "DNI";
        }

        public struct ConfiguracionGeneral
        {
            public struct Pantalla
            {
                public const string EficienciaCalado = "EficienciaCalado";
                public const string AFIP = "AFIP";
                public const string PreLote = "PreLote";
                public const string EstadoDeCallePostCalado = "EstadoDeCallePostCalado";
                public const string EstadoDeCallePreCalado = "EstadoDeCallePreCalado";
                public const string EstadoPlayaInterna = "EstadoPlayaInterna";
                public const string EstadoVolcadoras = "EstadoVolcadoras";
                public const string EstablecimientoPantalla = "Establecimiento";
                public const string TableroComandoLogistica = "TableroComandoLogistica";
                public const string TableroComandoPuerto = "TableroComandoPuerto";
                public const string IngresarOrdenCargaInternaFason = "IngresarOrdenCargaInternaFason";
                public const string CrearCartaPorteByPass = "CrearCartaPorteByPass";
                public const string PesadaBruto = "PesadaBruto";
                public const string CargaDeCupo = "CargaDeCupo";
                public const string ServicioSap = "ServicioSap";
                public const string ServicioOperaciones = "ServicioOperaciones";
                public const string PagoTasaMunicipal = "PagoTasaMunicipal";
                public const string ConsultaDataAgroVisec = "ConsultaDataAgroVisec";
                public const string MarcaSustentable = "MarcaSustentable";
                public const string MOAOperacionesListadoTicketPesada = "MOAOperaciones.ListadoTicketPesada";
                public const string BalanzaACero = "BalanzaACero";
                public const string LimpiarCacheCartaPorte = "LimpiarCacheCartaPorte";
                public const string Cardless = "Cardless";
            }

            public struct Cardless
            {
                public const string MaxSustituciones = "MaxSustituciones";
                public const string SustitucionActiva = "SustitucionActiva";
            }

            public struct ContingenciaPesosExcedidos
            {
                public const string CodigosWorkflows = "CodigosWorkflowParaContingenciaPesosExcedidos";
            }

            public struct EficienciaCalado
            {
                public const string EficienciaCalles = "EficienciaCalles";
                public const string HorarioTurno = "HorarioTurno";
            }

            public struct AFIP
            {
                public const string ConsultasParalelas = "ConsultasParalelas";
            }

            public struct PreLote
            {
                public const string HorarioNocturnoDesde = "HorarioNocturnoDesde";
                public const string HorarioNocturnoHasta = "HorarioNocturnoHasta";
            }

            public struct PostCalado
            {
                public const string CartelLedPostCalado = "CartelLedPostCalado";
            }

            public struct PreCalado
            {
                public const string CartelLedCalador = "CartelLedCalador";
                public const string LimiteFilasLlamadas = "LimiteFilasLlamadas";
            }

            public struct PreBalanza
            {
                public const string CartelLedPreBalanza = "CartelLedPreBalanza";
                public const string LlamadoAutomatico = "LlamadoAutomaticoPreBalanza";
                public const string VariablePredeterminadaPasoPrioritaria = "02";
            }

            public struct Volcadoras
            {
                public const string CartelLedIntervalo = "CartelLedIntervalo";
            }

            public struct LlamadoAutomatico
            {
                public const string Granos = "LlamadoAutomaticoGeneralGranos";
                public const string PreBalanza = "LlamadoAutomaticoPreBalanza";
                public const string NoGranos = "LlamadoAutomaticoGeneralNoGranos";
            }

            public struct ImpresionReciboMunicipal
            {
                public const string Actividad = "ImpresionReciboMunicipal";
                public const string MaterialesPagoRealizado = "MaterialesPagoRealizado";
            }

            public struct CNRT
            {
                public const string CNRTDummy = "CNRTDummy";
                public const string VehiculoDummy = "VehiculoDummy";
            }

            public struct CrearCartaPorteByPass
            {
                public const string Provincia = "Provincia";
                public const string Localidad = "Localidad";
                public const string Planta = "Planta";
                public const string Centro = "Centro";
                public const string WorkFlowEgreso = "WorkFlowEgreso";
                public const string TipoComercialEgreso = "TipoComercialEgreso";
            }

            public struct PagoTasaMunicipal
            {
                public const string NumeroDiasParaInicioBusqueda = "NumeroDiasParaInicioBusqueda";
                public const string PermitirBloqueoDeIngreso = "PermitirBloqueoDeIngreso";
            }

            public struct CargaDeCupo
            {
                public const string MaterialExcepcionFastPass = "MaterialExcepcionFastPass";
                public const string OmitirValidacionDataAgroVisec = "OmitirValidacionDataAgroVisec";
            }

            public struct Servicios
            {
                public const string OperacionesDummy = "OperacionesDummy";
                public const string OperacionesDummyResponse = "OperacionesDummyResponse";
                public const string SapDummy = "SapDummy";
                public const string SapDummyResponse = "SapDummyResponse";
            }

            public struct ConsultaDataAgroVisec
            {
                public const string Activo = "Activo";
                public const string DummyActivo = "DummyActivo";
                public const string DummyDatosCupoSapRespose = "DummyDatosCupoSapRespose";
            }

            public struct MarcaSustentable
            {
                public const string ImagenSustentableBase64 = "ImagenSustentableBase64";
                public const string PosicionImagenSustentableX = "PosicionImagenSustentableX";
                public const string PosicionImagenSustentableY = "PosicionImagenSustentableY";
            }
            
            public struct MOAOperacionesListadoTicketPesada
            {
                public const string TiposComerciales = "TiposComerciales";
            }

            public struct BalanzaACero
            {
                public const string TiempoEsperaEntreIntentos = "TiempoEsperaEntreIntentos";
            }
            
            public struct CartaPorteElectronica
            {
                public const string DiasLimiteDeBusqueda = "DiasLimiteDeBusqueda";
                public const string DiasInicioDeBusquedaDeRecorrido = "DiasInicioDeBusquedaDeRecorrido";

            }
        }

        public struct EtapaWorkflow
        {
            public const string Visteo = "Visteo";
            public const string ConfirmacionCargaDescarga = "ConfirmacionCargaDescarga";
            public const string PesadaBruto = "PesadaBruto";
            public const string PesadaTara = "PesadaTara";
            public const string OrdenCargaInterna = "IngresarOrdenCargaInternaFason";
            public const string MaterialNoProductivo = "IngresarOrdenCargaInterna";
            public const string IngresarOrdenCargaFas = "IngresarOrdenCargaFas";
            public const string EnTransito = "EnTransito";
        }

        public struct CartelTramaPare
        {
            public const string LlamadoAutomaticoVolcadoras = "02";
        }

        public struct Caladores
        {
            public const string CodigoCalador1 = "CAL1";
            public const string CodigoCalador2 = "CAL2";
            public const string NombreCalador1 = "Calador 1";
            public const string NombreCalador2 = "Calador 2";
        }

        public struct Proveedores
        {
            public const string CuitMolinos = "30-71511877-3";
        }

        public struct TipoCP
        {
            public const int CPCamion = 284;
            public const int CPTren = 286;
        }

        public struct ControlRecorrido
        {
            public struct Actividades
            {
                public const string ActividadDG = "Request Alta de CPE DG";
                public const string Actividad = "Request Alta de CPE";
            }

            public struct Mensajes
            {
                public const string Mensaje = "Automatico";
                public const string RequestAltaDG = "Request Alta de CPEDG";
                public const string ResponseAltaDG = "Response Alta de CPEDG";
                public const string RequestAnularCPE = "Request Anular CPE";
                public const string ResponseAnularCPE = "Response Anular CPE";
            }
        }

        public struct SAP
        {
            public const string TipoReventaComisionista = "C";
            public const string TipoReventaRemitente = "R";
        }

        public struct MaterialPagoRealizado
        {
            public const string BiodiselAgranel = "99319";
            public const string AceiteGirasolCrudoSAP = "94687";
            public const string AceiteSojaCrudoGranelSAP = "94705";
            public const string SojaSAP = "19908017";
        }

        public struct TipoSoja
        {
            public const string Sustentable = "sust";
            public const string EPA = "epa";
        }

        public struct Centro
        {
            public const string CodigoSAPSanLorenzo = "1029";
            public const int IdSanLorenzo = 5;
        }

        public struct CodigosSAP
        {
            public const string Girasol = "19908018";
        }

        public struct PuestoComandoPuerto
        {
            public const int ValorPorDefectoCalle = 3;
            public const int ValorPorDefectoAlmacen = 0;
        }

        public struct PuestoComando
        {
            public const string Value = "PuestoComando";
        }

        public struct TipoVariedadMaterial
        {
            public const string EPA = "EPA";
            public const string Sustentable = "SUS";
            public const string Importacion = "IMP";
            public const string Estandar = "EST";
            public const string EUDR = "EUDR";
            public const string EPAyEUDR = "EPAEUDR";
            public const string ImportacionACA = "IMPOACA";
            public const string ImportacionTPR = "IMPOTPR";
        }

        public struct Job
        {
            public const string LlamarAutomatismoGrano = "JobLlamarLlamadoAutomaticoGranos";
            public const string LlamarAutomatismoNoGrano = "JobLlamarLlamadoAutomaticoNoGranos";
            public const string SincronizarMOAPayEstadoDePagos = "SincronizarMOAPayEstadoDePagos";
            public const string SincronizarMOAPayCPE = "SincronizarMOAPayCPE";
            public const string SincronizarEstadoTransmisionVisec = "SincronizarEstadoTransmisionVisec";
            public const string SincronizarMOAPayOperacionesFason = "SincronizarMOAPayOperacionesFason";
            public const string SincronizarMOAPayOperacionesFas = "SincronizarMOAPayOperacionesFas";
            public const string SincronizarMOAPayOperacionesResiduos = "SincronizarMOAPayOperacionesResiduos";
            
            public const string SincronizarBandaHorariaStopRechazados = "SincronizarBandaHorariaStopRechazados";

            public const string DefaultCronExpressionForSincronizarMOAPayEstadoDePagos = "0 */30 * * * *"; //cada 30 minutos
            public const string VerificarHealthCheckMOAPayHealth = "VerificarHealthCheckMOAPayHealth";
            
            public const string CachearCpeAFIPSanLorenzo = "CachearCpeAFIPSanLorenzo";
            public const string CachearCpeAFIPPorCentros = "CachearCpeAFIPPorCentros";
            public const string ActualizarCacheCpeAFIPSanLorenzo = "ActualizarCacheCpeAFIPSanLorenzo";
            public const string LimpiarCacheCartaPorteElectronicaDocumentosIngresados = "LimpiarCacheCartaPorteElectronicaDocumentosIngresados";
            public const string LimpiarCacheCartaPorteElectronicaDocumentosNoIngresados = "LimpiarCacheCartaPorteElectronicaDocumentosNoIngresados";
            public const string DefaultCronExpressionForCachearCpeAFIPSanLorenzo = "0 */15 * * * *";
            public const string DefaultCronExpressionForActualizarCacheCpeAFIPSanLorenzo = "0 0 * * * *";
            public const string DefaultCronExpressionForCachearCpeAFIPPorCentros = "0 0 * * * *";
            public const string DefaultCronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosIngresados = "0 0 * * * *";
            public const string DefaultCronExpressionForLimpiarCacheCartaPorteElectronicaDocumentosNoIngresados = "0 0 3 * * *";
        }

        public struct AutomatismoTipoLlamado
        {
            public const string PorFila = "PFL";
            public const string UnoAUno = "1A1";
            public const string PaseDirecto = "PDR";
        }

        public struct WorkFlow
        {
            public const string workflowFason = "SLO.EgresoClienteFason";
            public const string workflowFasonSinFlete = "SLO.EgresoClienteFasonSinFlete";
            public const string workflowMaterialNoProductivo = "SLO.EgresoMaterialNoProductivo";
            public const string workflowVentaFas = "SLO.EgresoPorVentasFAS";
            public const string workflowExportacionFCA = "1029-EgresoPorExportacionFCA";
            public const string workflowIngresoImportacion = "1029-IngresoPorImpoGranos";
            public const string workflowRedespacho = "1029-IngresoPorRedespachoDeGranosCaladaExterna";
        }

        public struct Excepciones
        {
            public const string SecuenciaMultiplesElementos = "La secuencia contiene más de un elemento";
        }

        public struct CuitCliente
        {
            public const string Nestle = "30-54676404-0";
        }

        public struct TipoBalanzada
        {
            public const string Inicio = "inicio";
            public const string Fin = "fin";
            public const string Balanzada = "balanzada";
            public const string Error = "error";
            public const string InicioError = "inicioError";
            public const string Error41 = "error41";
            public const string Error44 = "error44";
            public const string FinError = "finError";
        }

        public struct TipoDocEnvioUrenport
        {
            public const string CartaPorteUrenport = "1";
            public const string CertificacionHojaDeRutaCartaPorte = "2";
        }

        public struct Contingencia
        {
            public const string VisecCaido = "VisecCaido";
            public const string PayCaido = "PayCaido";
        }

        public struct ClasificacionCategorias
        {
            public const string OPERADOR = "OPERADOR";
            public const string PRODUCTOR = "PRODUCTOR";
        }

        public static class MOAPay
        {
            public static class Filtros
            {
                public const string SI = "S";
                public const string NO = "N";
                public const string TODO = "T";

                public const string FECHAPAGO = "P";
            }

            public static class TipoDocumento
            {
                public const string REMITO = "Remito";
                public const string CTG = "CTG";
            }

            public static class TipoDeVehiculo
            {
                public const string COMUN = "C";
                public const string ESCALABLE = "E";
            }

            public static class TipoDeRemito
            {
                public const string Fason = "F";
                public const string Fas = "S";
                public const string Residuos = "I";
            }

            public static class Codigos
            {
                public const string CodigoDiferenciaDePago = "DP";
            }

            public static class MotivosDeExcepciones
            {
                public const string ExcepcionPorMaterialYCentro = "EXCEPCIÓN POR MATERIAL Y CENTRO";
                public const string ExcepcionPorPatente = "EXCEPCIÓN POR PATENTE";
                public const string ExcepcionPorPatenteYDocumento = "EXCEPCIÓN POR PATENTE Y DOCUMENTO";
                public const string ExcepcionPorSojaImpo = "EXCEPCIÓN POR SOJA IMPO";
                public const string ExcepcionPorPago24Hrs = "EXCEPCIÓN POR PAGO 24 HRS";
                public const string ExcepcionPorRecorridoReingreso = "EXCEPCIÓN POR REINGRESO";
            }
            public static class NombreUsuario
            {
                public const string SistemaScato = "SistemaScato";
            }
        }
    }
}