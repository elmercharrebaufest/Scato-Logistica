namespace Molinos.Scato.Dominio
{
    public static class Constantes
    {
        public struct ValoresPorDefecto
        {
            public const string CupoGenerico = "MOL1111/11111111";
            public const long CuitMOA = 30715118773;
            public const string NumeroRemitoGenerico = "1111-11111111";
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
        }

        public struct AFIPCodigoDeError
        {
            public const string NoExistenSolicitudes = "800";
        }

        public struct AsignacionDeEstablecimientoRangos
        {
            public const int Desde = 90000;
            public const int Hasta = 99998;
        }

        public struct TipoDocumentoChofer
        {
            public const string Cuit = "CUI";
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
            }

            public struct Volcadoras
            {
                public const string CartelLedIntervalo = "CartelLedIntervalo";
            }

        }

        public struct EtapaWorkflow
        {
            public const string Visteo = "Visteo";
            public const string ConfirmacionCargaDescarga = "ConfirmacionCargaDescarga";
            public const string PesadaBruto = "PesadaBruto";
            public const string PesadaTara = "PesadaTara";
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

        public struct DatosDummy
        {
            public const string DestinoPlanta = "3191";
            public const string DestinoDomicilioTipo = "1";
            public const string DestinoDomicilioOrden = "1";
        }

        public struct DerivadoGranario
        {
            public const int TipoDomicilioFiscal = 1;
            public const int TipoDomicilioPlanta = 3;
        }

        public struct SAP
        {
            public const string TipoReventaComisionista = "C";
            public const string TipoReventaRemitente = "R";
        }
    }
}