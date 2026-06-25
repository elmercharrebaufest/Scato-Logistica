using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Helpers
{
    public class WorkflowCartaPorteHelper
    {
        public static EscenarioWorkflowCartaPorte ObtenerEscenario(
            string codigoSapTitularCartaPorte,
            string codigoSapRemitenteComercial,
            string codigoSapDestino,
            string codigoSapDestinatario,
            string codigoSapMolinosAgro,
            string codigoSapMrp,
            string codigoEstablecimiento,
            string remitenteComercialVentaSecundariaCuit = null)
        {
            var titular = Normalizar(codigoSapTitularCartaPorte);
            var remitenteComercial = Normalizar(codigoSapRemitenteComercial);
            var destino = Normalizar(codigoSapDestino);
            var destinatario = Normalizar(codigoSapDestinatario);
            var codigoMolinos = Normalizar(codigoSapMolinosAgro);
            var codigoMrp = Normalizar(codigoSapMrp);
            var establecimiento = Normalizar(codigoEstablecimiento);
            var cuitVentaSecundaria = Normalizar(remitenteComercialVentaSecundariaCuit);

            var esTitularMrpConDestinoSanLorenzo = SonIguales(titular, codigoMrp)
                && SonIguales(destino, Constantes.Centro.CodigoSAPSanLorenzo);
            var esCasoCompraMrp = esTitularMrpConDestinoSanLorenzo && SonIguales(destinatario, codigoMrp);
            var esCasoRedespachoMrp = esTitularMrpConDestinoSanLorenzo && SonIguales(destinatario, codigoMolinos);
            var esCasoRedespachoPorTitularMolinosAgro = SonIguales(titular, codigoMolinos)
                && (string.IsNullOrEmpty(remitenteComercial) || SonIguales(remitenteComercial, codigoMolinos));
            var esCasoImportacion = SonIguales(titular, Constantes.ValoresPorDefecto.CodigoSapTPR)
                || (SonIguales(titular, Constantes.ValoresPorDefecto.CodigoSapACA)
                    && SonIguales(establecimiento, Constantes.ValoresPorDefecto.EstablecimientoACA)
                    && (SonIguales(remitenteComercial, codigoMolinos)
                        || SonIguales(cuitVentaSecundaria, Constantes.Proveedores.CuitMolinos)));

            if (esCasoCompraMrp)
                return EscenarioWorkflowCartaPorte.Compra;

            if (esCasoRedespachoMrp || esCasoRedespachoPorTitularMolinosAgro)
                return EscenarioWorkflowCartaPorte.Redespacho;

            if (esCasoImportacion)
                return EscenarioWorkflowCartaPorte.Importacion;

            if (!string.IsNullOrEmpty(titular))
                return EscenarioWorkflowCartaPorte.Compra;

            return EscenarioWorkflowCartaPorte.Indeterminado;
        }

        private static string Normalizar(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor.Trim();
        }

        private static bool SonIguales(string valor1, string valor2)
        {
            return !string.IsNullOrEmpty(valor1)
                && !string.IsNullOrEmpty(valor2)
                && valor1 == valor2;
        }
    }
}
