using System.Collections.Generic;
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
    public class VariedadPorVinedoControllerTest
    {
        private VariedadPorVinedoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<VariedadPorVinedoDto> vinedos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new VariedadPorVinedoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            vinedos = new List<VariedadPorVinedoDto>
                {
                    new VariedadPorVinedoDto
                        {
                            Id = 1,
                            Cosecha = "2010"
                        },
                    new VariedadPorVinedoDto
                        {
                            Id = 1,
                            Cosecha = "2011"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoVariedadPorVinedo(It.IsAny<int>(),It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VariedadPorVinedoDto>(vinedos, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });

            var result = target.Index(1) as ViewResult;
            IEnumerable<VariedadPorVinedoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,1 }));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoVariedadPorVinedo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VariedadPorVinedoDto>(vinedos, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });

            var result = target.Listar(1) as ViewResult;
            IEnumerable<VariedadPorVinedoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 1 }));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });
            
            var result = target.Crear(1) as ViewResult;
            List<SelectListItem> variedades = target.ViewBag.Variedades;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(variedades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearVariedadPorVinedo>()))
                .Returns(new Resultado());

            var vinedosDto = new VariedadPorVinedoDto
                {
                    Id = 1,
                    Cosecha = "2010"
                };

            var result = target.Crear(vinedosDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoVariedadPorVinedo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VariedadPorVinedoDto>(vinedos, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });
            
            var resultado = new Resultado();
            resultado.Error("Error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearVariedadPorVinedo>())).Returns(resultado);

            var vinedoDto = new VariedadPorVinedoDto
                {
                Id = 1,
                Cosecha = "2010"
            };

            var result = target.Crear(vinedoDto) as ViewResult;

            List<SelectListItem> variedades = target.ViewBag.Variedades;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(variedades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoVariedadPorVinedo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VariedadPorVinedoDto>(vinedos, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });
            
            servRepositorioMock.Setup(s => s.ObtenerVariedadPorVinedo(It.IsAny<int>()))
                .Returns(new VariedadPorVinedoDto
                    {
                    Id = 1,
                    Cosecha = "2010"
                });

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> variedades = target.ViewBag.Variedades;

            Assert.NotNull(result.Model);
            Assert.That(variedades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarVariedadPorVinedo>()))
                .Returns(new Resultado());

            var bocaDestinoDto = new VariedadPorVinedoDto
                {
                Id = 1,
                Cosecha = "2010"
            };

            var result = target.Modificar(bocaDestinoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoVariedadPorVinedo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<VariedadPorVinedoDto>(vinedos, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto(), new VariedadDto(), new VariedadDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarVariedadPorVinedo>())).Returns(resultado);

            var bocaDestinoDto = new VariedadPorVinedoDto
                {
                Id = 1,
                Cosecha = "2010"
            };

            var result = target.Modificar(bocaDestinoDto) as ViewResult;

            List<SelectListItem> varieadades = target.ViewBag.Variedades;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(varieadades.Count(), Is.EqualTo(3));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarVariedadPorVinedo>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarVariedadPorVinedo>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
