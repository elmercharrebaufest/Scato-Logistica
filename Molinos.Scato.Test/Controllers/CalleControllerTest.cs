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
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CalleControllerTest
    {
        private CalleController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<CalleDto> calleDto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CalleController(null, servRepositorioMock.Object, servComandosMock.Object);

            calleDto = new List<CalleDto>
                {
                    new CalleDto
                        {
                            Id = 1,
                            Codigo = "Calle1",
                            Nombre = "Calle Nro 1",
                            CentroId = 1
                        },
                    new CalleDto
                        {
                            Id = 2,
                            Codigo = "Calle2",
                            Nombre = "Calle Nro 2",
                            CentroId = 1
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoCalle(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CalleDto>(calleDto, 1, 2, 2));

            var result = target.Index(datosUsuario, It.IsAny<string>()) as ViewResult;
            IEnumerable<CalleDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("Calle Nro 1"));
        }

        [Test]
        public void TestListar()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoCalle(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CalleDto>(calleDto, 1, 2, 2));

            var result = target.Listar(datosUsuario, It.IsAny<string>()) as ViewResult;
            IEnumerable<CalleDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("Calle Nro 1"));
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCalle>()))
                .Returns(new Resultado());

            var result = target.Crear(calleDto[0], datosUsuario) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCalle>())).Returns(resultado);

            var result = target.Crear(calleDto[0], datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            var datoUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ObtenerCalle(It.IsAny<int>()))
                .Returns(new CalleDto()
                    {
                        Id = 1,
                        Nombre = "Calle1",
                        Codigo = "C1",
                        CentroId = 1
                    });

            var result = target.Modificar(1, datoUsuario) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCalle>()))
                .Returns(new Resultado());

            var tipoDto = new CalleDto()
                {
                    Id = 1,
                    Codigo = "C1",
                    Nombre = "Calle 1",
                    CentroId = 1
                };

            var result = target.Modificar(tipoDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCalle>())).Returns(resultado);

            var tipoDto = new CalleDto()
            {
                Id = 1,
                Codigo = "C1",
                Nombre = "Calle 1",
                CentroId = 1
            };

            var result = target.Modificar(tipoDto, datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCalle>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCalle>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
