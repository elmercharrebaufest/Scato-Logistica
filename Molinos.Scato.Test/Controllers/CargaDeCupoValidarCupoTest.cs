using System.Reflection;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
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
    // Cubre ValidarCupo: bypass cuando no requiere/valida cupo o es egreso a cliente destinatario /
    // cupo no MOL, y el caso donde el cupo ya está asignado a otra CP (deja la CP en Pendiente).
    [TestFixture]
    public class CargaDeCupoValidarCupoTest
    {
        private CargaDeCupoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private DatosUsuario datosUsuario;
        private MethodInfo metodoValidarCupo;

        [SetUp]
        public void SetUp()
        {
            var log = new NullLogger();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            var firmaMock = new Mock<IFirmaProvider>();
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

            metodoValidarCupo = typeof(CargaDeCupoController).GetMethod(
                "ValidarCupo", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private bool InvocarValidarCupo(CartaPorteDto orden, bool esIngreso)
        {
            return (bool)metodoValidarCupo.Invoke(target, new object[] { orden, datosUsuario, esIngreso });
        }

        [Test]
        public void CuandoNoRequiereCupo_EsValidoSinConsultarAlServicio()
        {
            var orden = new CartaPorteDto { RequiereCupo = false, ValidarCupo = true, Cupo = "MOL1111/22222222" };

            var esValido = InvocarValidarCupo(orden, esIngreso: true);

            Assert.IsTrue(esValido);
            servRepositorioMock.Verify(s => s.ValidarCupo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoNoDebeValidarCupo_EsValidoSinConsultarAlServicio()
        {
            var orden = new CartaPorteDto { RequiereCupo = true, ValidarCupo = false, Cupo = "MOL1111/22222222" };

            var esValido = InvocarValidarCupo(orden, esIngreso: true);

            Assert.IsTrue(esValido);
            servRepositorioMock.Verify(s => s.ValidarCupo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoEsEgresoAClienteDestinatario_EsValidoSinConsultarAlServicio()
        {
            var orden = new CartaPorteDto
            {
                RequiereCupo = true,
                ValidarCupo = true,
                EsClienteDestinatario = true,
                Cupo = "MOL1111/22222222"
            };

            var esValido = InvocarValidarCupo(orden, esIngreso: false);

            Assert.IsTrue(esValido);
            servRepositorioMock.Verify(s => s.ValidarCupo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoEsEgresoConCupoQueNoEmpiezaConMOL_EsValidoSinConsultarAlServicio()
        {
            var orden = new CartaPorteDto
            {
                RequiereCupo = true,
                ValidarCupo = true,
                EsClienteDestinatario = false,
                Cupo = "OTRO1111"
            };

            var esValido = InvocarValidarCupo(orden, esIngreso: false);

            Assert.IsTrue(esValido);
            servRepositorioMock.Verify(s => s.ValidarCupo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoElCupoYaEstaAsignado_NoEsValido()
        {
            var orden = new CartaPorteDto
            {
                RequiereCupo = true,
                ValidarCupo = true,
                Cupo = "MOL1111/22222222",
                NroCartaPorte = "000000000001"
            };
            servRepositorioMock.Setup(s => s.ValidarCupo(orden.Cupo, datosUsuario.CentroId, orden.NroCartaPorte))
                .Returns(new ValidarCupoDto { YaAsignado = true, MensajeError = "Cupo ya asignado a otra CP" });

            var esValido = InvocarValidarCupo(orden, esIngreso: true);

            Assert.IsFalse(esValido);
        }

        [Test]
        public void CuandoElCupoNoEstaAsignado_EsValido()
        {
            var orden = new CartaPorteDto
            {
                RequiereCupo = true,
                ValidarCupo = true,
                Cupo = "MOL1111/22222222",
                NroCartaPorte = "000000000001"
            };
            servRepositorioMock.Setup(s => s.ValidarCupo(orden.Cupo, datosUsuario.CentroId, orden.NroCartaPorte))
                .Returns(new ValidarCupoDto { YaAsignado = false });

            var esValido = InvocarValidarCupo(orden, esIngreso: true);

            Assert.IsTrue(esValido);
        }

        [Test]
        public void CuandoEsEgresoConCupoMOLYNoEsClienteDestinatario_ConsultaAlServicio()
        {
            var orden = new CartaPorteDto
            {
                RequiereCupo = true,
                ValidarCupo = true,
                EsClienteDestinatario = false,
                Cupo = "MOL1111/22222222",
                NroCartaPorte = "000000000002"
            };
            servRepositorioMock.Setup(s => s.ValidarCupo(orden.Cupo, datosUsuario.CentroId, orden.NroCartaPorte))
                .Returns(new ValidarCupoDto { YaAsignado = false });

            var esValido = InvocarValidarCupo(orden, esIngreso: false);

            Assert.IsTrue(esValido);
            servRepositorioMock.Verify(s => s.ValidarCupo(orden.Cupo, datosUsuario.CentroId, orden.NroCartaPorte), Times.Exactly(1));
        }
    }
}
