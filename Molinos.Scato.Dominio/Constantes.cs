namespace Molinos.Scato.Dominio
{
    public static class Constantes
    {
        public struct ValoresPorDefecto
        {
            public const string CupoGenerico = "MOL1111/11111111";
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
        }

        public struct EtapaWorkflow
        {
            public const string Visteo = "Visteo";
            public const string ConfirmacionCargaDescarga = "ConfirmacionCargaDescarga";
        }
    }
}
