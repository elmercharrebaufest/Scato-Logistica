using System;
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
    public class SuplenciaControllerTest
    {
        private SuplenciaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<SuplenciaDto> suplencias;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new SuplenciaController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            suplencias = new List<SuplenciaDto>
                {
                    new SuplenciaDto
                        {
                            Id = 1,
                            FechaDesde = DateTime.Today.AddDays(-7),
                            FechaHasta = DateTime.Today,
                            UsuarioASuplantarId = 10,
                            UsuarioSuplenteId = 1
                        },
                    new SuplenciaDto
                        {
                            Id = 2,
                            FechaDesde = DateTime.Today.AddDays(-7),
                            FechaHasta = DateTime.Today,
                            UsuarioASuplantarId = 20,
                            UsuarioSuplenteId = 2
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoSuplencias(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<SuplenciaDto>(suplencias, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<SuplenciaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoSuplencias(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<SuplenciaDto>(suplencias, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<SuplenciaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoSuplencias("2", It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<SuplenciaDto>(new List<SuplenciaDto> { suplencias[1] }, 1, 1, 1));

            const string filter = "2";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<SuplenciaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(2));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearSuplencia>()))
                .Returns(new Resultado());

            var result = target.Crear(suplencias[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servRepositorioMock.Setup(s => s.ListarUsuarios())
                .Returns(new List<UsuarioDto>{ new UsuarioDto {NombreUsuario = "Usuario 1"}, new UsuarioDto {NombreUsuario = "Usuario 2"}});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearSuplencia>())).Returns(resultado);

            var result = target.Crear(suplencias[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarUsuarios())
                .Returns(new List<UsuarioDto> { new UsuarioDto { NombreUsuario = "Usuario 1" }, new UsuarioDto { NombreUsuario = "Usuario 2" } });
            servRepositorioMock.Setup(s => s.ObtenerSuplencia(1))
                .Returns(suplencias[0]);

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarSuplencia>()))
                .Returns(new Resultado());

            var suplenciaDto = new SuplenciaDto
            {
                Id = 1,
                UsuarioASuplantarId = 11,
                UsuarioSuplenteId = 1
            };

            var result = target.Modificar(suplenciaDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarUsuarios())
                .Returns(new List<UsuarioDto> { new UsuarioDto { NombreUsuario = "Usuario 1" }, new UsuarioDto { NombreUsuario = "Usuario 2" } });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarSuplencia>()))
                .Returns(new Resultado());

            var suplenciaDto = new SuplenciaDto
            {
                Id = 1,
                UsuarioASuplantarId = 11,
                UsuarioSuplenteId = 1
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(suplenciaDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarSuplencia>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
