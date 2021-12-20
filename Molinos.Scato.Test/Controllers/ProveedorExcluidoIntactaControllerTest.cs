using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ProveedorExcluidoIntactaControllerTest
    {
        private ProveedorExcluidoIntactaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ProveedorExcluidoIntactaDto> ProveedorExcluidoIntactaDto;
        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            logger = new NullLogger();
            target = new ProveedorExcluidoIntactaController(logger, servRepositorioMock.Object, servComandosMock.Object);

            ProveedorExcluidoIntactaDto = new List<ProveedorExcluidoIntactaDto>
                {
                    new ProveedorExcluidoIntactaDto
                        {
                            Id = 1,
                            ProveedorId = 6,
                            RazonSocial = "Molinos Rio de la Plata"
                        },
                    new ProveedorExcluidoIntactaDto
                        {
                            Id = 2,
                            ProveedorId = 9,
                            RazonSocial = "Cerro Puerto"
                        }
                };
            //servRepositorioMock.Setup(x => x.ListarEmpresas()).Returns(new List<EmpresaDto>());
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoProveedorExcluidoIntacta(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ProveedorExcluidoIntactaDto>(ProveedorExcluidoIntactaDto, 1, 2, 2));

            var result = target.Index(It.IsAny<string>()) as ViewResult;
            IEnumerable<ProveedorExcluidoIntactaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].RazonSocial, Is.EqualTo("Molinos Rio de la Plata"));
        }

        [Test]
        public void TestListar()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoProveedorExcluidoIntacta(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ProveedorExcluidoIntactaDto>(ProveedorExcluidoIntactaDto, 1, 2, 2));

            var result = target.Listar(It.IsAny<string>()) as ViewResult;
            IEnumerable<ProveedorExcluidoIntactaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].RazonSocial, Is.EqualTo("Molinos Rio de la Plata"));
        }

        [Test]
        public void TestCrear()
        {
            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearProveedorExcluidoIntacta>()))
                .Returns(new Resultado());

            var result = target.Crear(ProveedorExcluidoIntactaDto[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearProveedorExcluidoIntacta>())).Returns(resultado);

            var result = target.Crear(ProveedorExcluidoIntactaDto[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarProveedorExcluidoIntacta>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarProveedorExcluidoIntacta>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
