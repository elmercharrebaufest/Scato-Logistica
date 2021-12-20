using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Helpers
{
    public static class FotoCamionHelper
    {
        public static string GenerarNombre(string codigoSapCentro, string numeroDocumentoIngreso, string patente, string proximaActividad, DateTime date, TipoVehiculo tipoVehiculo)
        {
            return codigoSapCentro + "-" + numeroDocumentoIngreso + (tipoVehiculo == TipoVehiculo.Tren ? "" : ("-" + patente ) + "-" + proximaActividad) + "-" + date.ToString("HHmmss");
        }

        public static string GenerarNombreBusqueda(string codigoSapCentro, string numeroDocumentoIngreso, string patente, string proximaActividad, TipoVehiculo tipoVehiculo)
        {
            return codigoSapCentro + "-" + numeroDocumentoIngreso + (tipoVehiculo == TipoVehiculo.Tren ? "" : ("-" + patente + "-" + proximaActividad));
        }

        public static string GenerarNombre(string codigoSapCentro, string numeroDocumentoIngreso, string patente)
        {
            return codigoSapCentro + "-" + numeroDocumentoIngreso + "-" + patente;
        }

        public static string GenerarNombreTemporal(string numeroDeTarjeta, DateTime date)
        {
            return numeroDeTarjeta + "-" + date.ToString("HHmmss");
        }
    }
}
