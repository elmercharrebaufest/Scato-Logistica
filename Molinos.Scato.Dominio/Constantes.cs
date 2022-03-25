namespace Molinos.Scato.Dominio
{
    public static class Constantes
    {
        public struct IntercomunicadorDireccion
        {
            public const string HaciaLaWeb = "2web";
            public const string DesdeLaWeb = "web2";
        }

        public struct NotificacionGrupos
        {
            public const string Intercomunicador = "Intercomunicador";
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
        }

        public struct ConfiguracionGeneral
        {
            public struct Pantalla
            {
                public const string EficienciaCalado = "EficienciaCalado";
                public const string AFIP = "AFIP";
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
        }
    }
}