using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using NUnit.Framework;

namespace Molinos.Scato.Test.Helpers
{
    [TestFixture]
    public class WorkflowCartaPorteHelperTests
    {
        private const string CodigoSapMolinos = "MOL001";
        private const string CodigoSapMrp = "MRP001";

        [Test]
        public void CuandoEsCasoCompraMrp_DebeRetornarCompra()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                CodigoSapMrp,
                "ANY",
                Constantes.Centro.CodigoSAPSanLorenzo,
                CodigoSapMrp,
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Compra, resultado);
        }

        [Test]
        public void CuandoEsCasoRedespachoMrp_DebeRetornarRedespacho()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                CodigoSapMrp,
                "ANY",
                Constantes.Centro.CodigoSAPSanLorenzo,
                CodigoSapMolinos,
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Redespacho, resultado);
        }

        [Test]
        public void CuandoTitularEsMolinosYRemitenteVacio_DebeRetornarRedespacho()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                CodigoSapMolinos,
                string.Empty,
                "X",
                "Y",
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Redespacho, resultado);
        }

        [Test]
        public void CuandoEsCasoImportacionPorTpr_DebeRetornarImportacion()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                Constantes.ValoresPorDefecto.CodigoSapTPR,
                "ANY",
                "X",
                "Y",
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Importacion, resultado);
        }

        [Test]
        public void CuandoEsCasoImportacionPorAca_DebeRetornarImportacion()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                Constantes.ValoresPorDefecto.CodigoSapACA,
                CodigoSapMolinos,
                "X",
                "Y",
                CodigoSapMolinos,
                CodigoSapMrp,
                Constantes.ValoresPorDefecto.EstablecimientoACA,
                Constantes.Proveedores.CuitMolinos);

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Importacion, resultado);
        }

        [Test]
        public void CuandoSoloHayTitularInformado_DebeRetornarCompra()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                "OTRO001",
                "REM001",
                string.Empty,
                string.Empty,
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Compra, resultado);
        }

        [Test]
        public void CuandoNoHayTitularInformado_DebeRetornarIndeterminado()
        {
            var resultado = WorkflowCartaPorteHelper.ObtenerEscenario(
                null,
                "REM001",
                string.Empty,
                string.Empty,
                CodigoSapMolinos,
                CodigoSapMrp,
                "123");

            Assert.AreEqual(EscenarioWorkflowCartaPorte.Indeterminado, resultado);
        }
    }
}
