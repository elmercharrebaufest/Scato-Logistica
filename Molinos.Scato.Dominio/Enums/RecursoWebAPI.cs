namespace Molinos.Scato.Dominio.Enums
{
    public static class RecursoWebAPI
    {
        public struct AFIP
        {
            public const string ConsultarCPEPendienteResolucion = "AFIPApi/ConsultarCPEPendienteResolucion";
            public const string ConfirmarDescargadoDestinoCPE = "AFIPApi/ConfirmarDescargadoDestinoCPE";
        }

        public struct MOAPAY
        {
            public const string ObtenerPagos = "MOAPayApi/ObtenerPagos";
            public const string InformarPagoComoConsumido = "MOAPayApi/InformarPagoComoConsumido";
            public const string CrearModificar = "MOAPayApi/CrearModificar";
        }
        
        public struct VISEC
        {
            public const string ImportacionCartaPorte = "VisecApi/ImportacionCartaPorte";
            public const string ExisteStockUP = "VisecApi/ExisteStockUnidadProductiva";
            public const string ExisteStockRUCA = "VisecApi/ExisteStockRUCA";
            public const string ConsultarEstadoImportacion = "VisecApi/ConsultarEstadoImportacion";
        }

        public struct STOP
        {
            public const string ValidarAcceso = "api/BandasHorarias/ValidarAcceso";
        }
    }
}