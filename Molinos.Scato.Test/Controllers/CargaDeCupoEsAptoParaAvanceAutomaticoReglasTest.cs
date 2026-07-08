using System.Reflection;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    // Cubre las reglas de negocio de EsAptoParaAvanceAutomatico (más allá de los intervinientes,
    // que ya están cubiertos por CargaDeCupoEsAptoParaAvanceAutomaticoTest): escenario Redespacho,
    // chofer con otro recorrido activo, y las reglas específicas de material EPA.
    [TestFixture]
    public class CargaDeCupoEsAptoParaAvanceAutomaticoReglasTest
    {
        private CargaDeCupoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IFirmaProvider> firmaMock;
        private DatosUsuario datosUsuario;
        private MethodInfo metodoEsAptoParaAvanceAutomatico;
        private const string CodigoSapMolinosAgro = "1000";

        [SetUp]
        public void SetUp()
        {
            var log = new NullLogger();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            firmaMock = new Mock<IFirmaProvider>();
            var servComandoMock = new Mock<IServicioComandos>();
            var listaMock = new Mock<IListaDeWorkflows>();
            var servicioSap = new Mock<ZSDWS_SCATO>();
            var servOrquestadorMock = new Mock<IServicioOrquestador>();
            var configuracionMock = new Mock<IConfiguracionProvider>();
            var factoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteService>>();
            var factoryNoProductivoMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>>();
            var factoryFasonMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService>>();
            var factoryFasMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>>();

            target = new CargaDeCupoController(log, servRepositorioMock.Object, servComandoMock.Object,
                listaMock.Object, servicioSap.Object, servOrquestadorMock.Object, configuracionMock.Object,
                firmaMock.Object, factoryMock.Object, factoryNoProductivoMock.Object,
                factoryFasonMock.Object, factoryFasMock.Object);

            datosUsuario = new DatosUsuario { CentroId = 1, NombreUsuario = "usuarioTest" };

            firmaMock.Setup(f => f.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = CodigoSapMolinosAgro });

            metodoEsAptoParaAvanceAutomatico = typeof(CargaDeCupoController).GetMethod(
                "EsAptoParaAvanceAutomatico", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private bool InvocarEsApto(CartaPorteDto orden)
        {
            return (bool)metodoEsAptoParaAvanceAutomatico.Invoke(
                target, new object[] { orden, datosUsuario, null });
        }

        [Test]
        public void CuandoEscenarioEsRedespacho_NoEsAptoParaAvanceAutomatico()
        {
            // Titular == Molinos Agro y sin remitente comercial => escenario Redespacho
            servRepositorioMock.Setup(s => s.ObtenerProveedor(1))
                .Returns(new ProveedorDto { CodigoSap = CodigoSapMolinosAgro });
            var orden = new CartaPorteDto { TitularCartaPorteId = 1, RtteComercialId = 0 };

            var esApto = InvocarEsApto(orden);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoEscenarioEsCompra_EsAptoSinEvaluarElRestoDeLasReglas()
        {
            // Titular con código SAP genérico (no MRP, no Molinos, no TPR/ACA) => escenario Compra (bypass)
            servRepositorioMock.Setup(s => s.ObtenerProveedor(1)).Returns(new ProveedorDto { CodigoSap = "99999" });
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
                .Returns(new OtroRecorridoDelChoferDto { Patente = "AAA111" }); // debería ignorarse por el bypass
            var orden = new CartaPorteDto
            {
                TitularCartaPorteId = 1,
                RtteComercialId = 0,
                Chofer = new ChoferDto { Id = 10 },
                TipoVehiculo = TipoVehiculo.Camión
            };

            var esApto = InvocarEsApto(orden);

            Assert.IsTrue(esApto);
        }

        private CartaPorteDto CrearOrdenEscenarioImportacion()
        {
            // Titular == CodigoSapTPR => escenario Importación, habilita la evaluación de chofer/EPA
            servRepositorioMock.Setup(s => s.ObtenerProveedor(1))
                .Returns(new ProveedorDto { CodigoSap = Constantes.ValoresPorDefecto.CodigoSapTPR });
            return new CartaPorteDto
            {
                TitularCartaPorteId = 1,
                RtteComercialId = 0,
                Chofer = new ChoferDto { Id = 10 },
                TipoVehiculo = TipoVehiculo.Camión
            };
        }

        [Test]
        public void CuandoChoferTieneOtroRecorridoActivoYNoEsTren_NoEsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(10))
                .Returns(new OtroRecorridoDelChoferDto { Patente = "AAA111" });

            var esApto = InvocarEsApto(orden);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoChoferTieneOtroRecorridoActivoPeroEsTren_EsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            orden.TipoVehiculo = TipoVehiculo.Tren;
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(10))
                .Returns(new OtroRecorridoDelChoferDto { Patente = "AAA111" });

            var esApto = InvocarEsApto(orden);

            Assert.IsTrue(esApto);
        }

        [Test]
        public void CuandoEsEPAEnSanLorenzoConMaterialSoja_NoEsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            orden.TipoVariedadCodigo = Constantes.TipoVariedadMaterial.EPA;
            orden.MaterialCodigoSap = Constantes.MaterialPagoRealizado.SojaSAP;
            datosUsuario.CentroId = Constantes.Centro.IdSanLorenzo;

            var esApto = InvocarEsApto(orden);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoEsEPAEnSanLorenzoConDestinatarioMolinos_NoEsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            orden.TipoVariedadCodigo = Constantes.TipoVariedadMaterial.EPA;
            orden.DestinatarioCuil = Constantes.Proveedores.CuitMolinos;
            datosUsuario.CentroId = Constantes.Centro.IdSanLorenzo;

            var esApto = InvocarEsApto(orden);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoEsEPAYTitularNoEsProveedorSustentable_NoEsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            orden.TipoVariedadCodigo = Constantes.TipoVariedadMaterial.EPA;
            servRepositorioMock.Setup(s => s.EsProveedorSustentable(orden.TitularCartaPorteId)).Returns(false);

            var esApto = InvocarEsApto(orden);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoEsEPAYTitularEsProveedorSustentable_EsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();
            orden.TipoVariedadCodigo = Constantes.TipoVariedadMaterial.EPA;
            servRepositorioMock.Setup(s => s.EsProveedorSustentable(orden.TitularCartaPorteId)).Returns(true);

            var esApto = InvocarEsApto(orden);

            Assert.IsTrue(esApto);
        }

        [Test]
        public void CuandoNoHayReglaEspecialQueLoBloquee_EsAptoParaAvanceAutomatico()
        {
            var orden = CrearOrdenEscenarioImportacion();

            var esApto = InvocarEsApto(orden);

            Assert.IsTrue(esApto);
        }
    }
}
