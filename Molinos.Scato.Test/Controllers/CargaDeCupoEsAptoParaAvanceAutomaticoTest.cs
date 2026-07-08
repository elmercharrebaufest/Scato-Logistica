using System.Collections.Generic;
using System.Reflection;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
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
    // Verifica que EsAptoParaAvanceAutomatico trate todos los intervinientes de
    // ClavesIntervinientesNoBloqueantes de la misma forma: no bloquean la consulta,
    // pero sí bloquean el avance automático (la CP queda en Pendiente).
    [TestFixture]
    public class CargaDeCupoEsAptoParaAvanceAutomaticoTest
    {
        private CargaDeCupoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IFirmaProvider> firmaMock;
        private DatosUsuario datosUsuario;
        private CartaPorteDto orden;
        private MethodInfo metodoEsAptoParaAvanceAutomatico;

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
            orden = new CartaPorteDto { TitularCartaPorteId = 1, RtteComercialId = 2 };

            metodoEsAptoParaAvanceAutomatico = typeof(CargaDeCupoController).GetMethod(
                "EsAptoParaAvanceAutomatico", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static IEnumerable<string> ClavesInterviniente()
        {
            var campo = typeof(CargaDeCupoController).GetField(
                "ClavesIntervinientesNoBloqueantes", BindingFlags.NonPublic | BindingFlags.Static);
            return (string[])campo.GetValue(null);
        }

        private bool InvocarEsApto(IDictionary<string, string> erroresIntervinientes)
        {
            return (bool)metodoEsAptoParaAvanceAutomatico.Invoke(
                target, new object[] { orden, datosUsuario, erroresIntervinientes });
        }

        [TestCaseSource(nameof(ClavesInterviniente))]
        public void CuandoHayErrorDeInterviniente_NoEsAptoParaAvanceAutomatico(string claveInterviniente)
        {
            var erroresIntervinientes = new Dictionary<string, string>
            {
                { claveInterviniente, "Proveedor no encontrado en SAP." }
            };

            var esApto = InvocarEsApto(erroresIntervinientes);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoErrorEsDePagadorFlete_NoEsAptoParaAvanceAutomatico()
        {
            var erroresIntervinientes = new Dictionary<string, string>
            {
                { Textos.CartaPorte_Transportista_Pagador_Flete, "Flete pagador no encontrado en SAP." }
            };

            var esApto = InvocarEsApto(erroresIntervinientes);

            Assert.IsFalse(esApto);
        }

        [Test]
        public void CuandoNoHayErroresDeIntervinientes_EvaluaElRestoDeLasReglas()
        {
            firmaMock.Setup(f => f.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "1000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.TitularCartaPorteId))
                .Returns(new ProveedorDto { CodigoSap = "2000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.RtteComercialId))
                .Returns(new ProveedorDto { CodigoSap = "3000" });

            var esApto = InvocarEsApto(erroresIntervinientes: null);

            Assert.IsTrue(esApto);
        }

        [Test]
        public void CuandoElDiccionarioDeErroresEsVacio_EvaluaElRestoDeLasReglas()
        {
            firmaMock.Setup(f => f.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "1000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.TitularCartaPorteId))
                .Returns(new ProveedorDto { CodigoSap = "2000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.RtteComercialId))
                .Returns(new ProveedorDto { CodigoSap = "3000" });

            var esApto = InvocarEsApto(new Dictionary<string, string>());

            Assert.IsTrue(esApto);
        }

        [Test]
        public void CuandoElErrorNoPerteneceAUnInterviniente_EvaluaElRestoDeLasReglas()
        {
            firmaMock.Setup(f => f.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "1000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.TitularCartaPorteId))
                .Returns(new ProveedorDto { CodigoSap = "2000" });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(orden.RtteComercialId))
                .Returns(new ProveedorDto { CodigoSap = "3000" });

            var erroresIntervinientes = new Dictionary<string, string>
            {
                { "ClaveNoRelacionadaAIntervinientes", "Otro error sin relación." }
            };

            var esApto = InvocarEsApto(erroresIntervinientes);

            Assert.IsTrue(esApto);
        }
    }
}
