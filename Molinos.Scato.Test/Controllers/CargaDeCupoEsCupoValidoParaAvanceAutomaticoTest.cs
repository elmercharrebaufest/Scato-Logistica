using System.Reflection;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    // Cubre los escenarios de EsCupoValidoParaAvanceAutomatico: cupo sustentable (EPA/EPA-EUDR)
    // y cupo genérico, ambos deben dejar la CP en Pendiente (no disparar el avance automático).
    [TestFixture]
    public class CargaDeCupoEsCupoValidoParaAvanceAutomaticoTest
    {
        private CargaDeCupoController target;
        private MethodInfo metodoEsCupoValido;

        [SetUp]
        public void SetUp()
        {
            var log = new NullLogger();
            var servRepositorioMock = new Mock<IServicioRepositorio>();
            var servComandoMock = new Mock<IServicioComandos>();
            var listaMock = new Mock<IListaDeWorkflows>();
            var servicioSap = new Mock<ZSDWS_SCATO>();
            var servOrquestadorMock = new Mock<IServicioOrquestador>();
            var configuracionMock = new Mock<IConfiguracionProvider>();
            var firmaMock = new Mock<IFirmaProvider>();
            var factoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteService>>();
            var factoryNoProductivoMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>>();
            var factoryFasonMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService>>();
            var factoryFasMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>>();

            target = new CargaDeCupoController(log, servRepositorioMock.Object, servComandoMock.Object,
                listaMock.Object, servicioSap.Object, servOrquestadorMock.Object, configuracionMock.Object,
                firmaMock.Object, factoryMock.Object, factoryNoProductivoMock.Object,
                factoryFasonMock.Object, factoryFasMock.Object);

            metodoEsCupoValido = typeof(CargaDeCupoController).GetMethod(
                "EsCupoValidoParaAvanceAutomatico", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private bool InvocarEsCupoValido(CargaDeCupoDto model, string tipoVariedadCodigo)
        {
            return (bool)metodoEsCupoValido.Invoke(target, new object[] { model, tipoVariedadCodigo });
        }

        [Test]
        public void CuandoEsCupoSustentableEPASinEUDR_NoEsValidoParaAvanceAutomatico()
        {
            var model = new CargaDeCupoDto { Especial = true, MaterialId = 4 };

            var esValido = InvocarEsCupoValido(model, Constantes.TipoVariedadMaterial.EPA);

            Assert.IsFalse(esValido);
        }

        [Test]
        public void CuandoEsCupoSustentableConEUDR_EsValidoParaAvanceAutomatico()
        {
            var model = new CargaDeCupoDto { Especial = true, MaterialId = 4 };

            var esValido = InvocarEsCupoValido(model, Constantes.TipoVariedadMaterial.EUDR);

            Assert.IsTrue(esValido);
        }

        [Test]
        public void CuandoEsCupoGenerico_NoEsValidoParaAvanceAutomatico()
        {
            var model = new CargaDeCupoDto
            {
                SinCupo = true,
                Cupo = Constantes.ValoresPorDefecto.CupoGenerico
            };

            var esValido = InvocarEsCupoValido(model, Constantes.TipoVariedadMaterial.Estandar);

            Assert.IsFalse(esValido);
        }

        [Test]
        public void CuandoTieneCupoRealAsignado_EsValidoParaAvanceAutomatico()
        {
            var model = new CargaDeCupoDto
            {
                SinCupo = false,
                Cupo = "MOL1111/22222222",
                Especial = false
            };

            var esValido = InvocarEsCupoValido(model, Constantes.TipoVariedadMaterial.Estandar);

            Assert.IsTrue(esValido);
        }

        [Test]
        public void CuandoNoEsEspecialYNoEsSinCupo_EsValidoParaAvanceAutomatico()
        {
            var model = new CargaDeCupoDto { Especial = false, SinCupo = false, MaterialId = 4 };

            var esValido = InvocarEsCupoValido(model, Constantes.TipoVariedadMaterial.EPA);

            Assert.IsTrue(esValido);
        }
    }
}
