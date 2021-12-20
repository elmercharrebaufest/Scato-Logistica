using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class VinedoPropioControllerTest
    {
        private VinedoPropioController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<VinedoPropioDto> vinedosPropios;
        private List<CuartelDto> cuarteles;
        private Mock<IFirmaProvider> configuracion;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            configuracion = new Mock<IFirmaProvider>();
            target = new VinedoPropioController(
                null, servRepositorioMock.Object, servComandosMock.Object, configuracion.Object);

            cuarteles = new List<CuartelDto>
                {
                    new CuartelDto
                        {
                           Id = 1,
                           Codigo = "5000"
                        },
                    new CuartelDto
                        {
                           Id = 2,
                           Codigo = "5001"
                        }
                };
            vinedosPropios = new List<VinedoPropioDto>
                {
                    new VinedoPropioDto
                        {
                            Id = 1,
                            NumeroINV = "1000",
                            Cuarteles = cuarteles,
                            Descripcion = "Viñedo1",
                            CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                            IngresosBrutos = "6556"
                        },
                    new VinedoPropioDto
                        {
                            Id = 2,
                            NumeroINV = "1001",
                            Descripcion = "Viñedo2",
                            CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                            IngresosBrutos = "7000"
                        }
                };
            configuracion.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "9950085862" });
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VinedoPropioDto>(vinedosPropios, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<VinedoPropioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VinedoPropioDto>(vinedosPropios, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<VinedoPropioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearVinedoPropio>()))
                .Returns(new Resultado());

            var result = target.Crear(vinedosPropios[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>())).Returns(new ListaPaginada<VinedoPropioDto>(vinedosPropios, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarZonas()).Returns(new List<ZonaDto>());
            servRepositorioMock.Setup(s => s.ListarSubZonasPorZona(It.IsAny<int>())).Returns(new List<SubZonaDto>());
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearVinedoPropio>())).Returns(resultado);

            var result = target.Crear(vinedosPropios[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()), Times.Exactly(0));
            servRepositorioMock.Verify(p => p.ListarZonas(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarSubZonasPorZona(It.IsAny<int>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VinedoPropioDto>(vinedosPropios, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarZonas()).Returns(new List<ZonaDto>());
            servRepositorioMock.Setup(s => s.ListarSubZonasPorZona(It.IsAny<int>())).Returns(new List<SubZonaDto>());
            servRepositorioMock.Setup(s => s.ObtenerVinedoPropio(It.IsAny<int>()))
                .Returns(new VinedoPropioDto
                {
                    Id = 1,
                    NumeroINV = "1000",
                    Cuarteles = cuarteles,
                    Descripcion = "Viñedo1",
                    CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                    IngresosBrutos = "7000"
                });

            var result = target.Modificar(1) as ViewResult;

            servRepositorioMock.Verify(p => p.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()), Times.Exactly(0));
            servRepositorioMock.Verify(p => p.ListarZonas(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarSubZonasPorZona(It.IsAny<int>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarVinedoPropio>()))
                .Returns(new Resultado());

            var vinedo = new VinedoPropioDto
            {
                Id = 1,
                NumeroINV = "1000",
                Cuarteles = cuarteles,
                Descripcion = "Viñedo1",
                CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                IngresosBrutos = "7000"
            };

            var result = target.Modificar(vinedo) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VinedoPropioDto>(vinedosPropios, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarZonas()).Returns(new List<ZonaDto>());
            servRepositorioMock.Setup(s => s.ListarSubZonasPorZona(It.IsAny<int>())).Returns(new List<SubZonaDto>());
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarVinedoPropio>())).Returns(resultado);

            var vinedo = new VinedoPropioDto
            {
                Id = 1,
                NumeroINV = "1000",
                Cuarteles = cuarteles,
                Descripcion = "Viñedo1",
                CuartelesJson = "[{\"Id\":1,\"Codigo\":\"5000\",\"Activo\":true},{\"Id\":2,\"Codigo\":\"5001\",\"Activo\":false}]",
                IngresosBrutos = "7000"
            };

            var result = target.Modificar(vinedo) as ViewResult;

            servRepositorioMock.Verify(p => p.ListarVinedoPropio(It.IsAny<string>(), It.IsAny<Paginacion>()), Times.Exactly(0));
            servRepositorioMock.Verify(p => p.ListarZonas(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarSubZonasPorZona(It.IsAny<int>()), Times.Exactly(1));
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestEliminar()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarVinedoPropio>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        [Test]
        public void TestEliminarInvalido()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarVinedoPropio>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}