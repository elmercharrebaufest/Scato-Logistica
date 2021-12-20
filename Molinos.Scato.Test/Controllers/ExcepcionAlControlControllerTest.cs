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
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ExcepcionAlControlControllerTest
    {
        private ExcepcionAlControlController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ExcepcionAlControlDto> excepciones;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ExcepcionAlControlController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            excepciones = new List<ExcepcionAlControlDto>
                {
                    new ExcepcionAlControlDto
                        {
                            Id = 1,
                            TransportistaId = 1,
                            MaterialId = 2,
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            CentroNombre = "c1"
                        },
                    new ExcepcionAlControlDto
                        {
                            Id = 2,
                            TransportistaId = 1,
                            MaterialId = 2,
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            CentroNombre = "c2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarExcepcionesAlControl(It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<ExcepcionAlControlDto>(excepciones, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<ExcepcionAlControlDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarExcepcionesAlControl(It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<ExcepcionAlControlDto>(excepciones, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<ExcepcionAlControlDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarTransportistas())
                .Returns(new List<TransportistaDto> { new TransportistaDto { RazonSocial = "a" }, new TransportistaDto() });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Descripcion = "centro1" }, new CentroDto { Descripcion = "centro2" }, new CentroDto { Descripcion = "centro3" } });

            var result = target.Crear() as ViewResult;
            List<SelectListItem> centros = target.ViewBag.Centros;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(centros.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionAlControl>()))
                .Returns(new Resultado());

            var excepcionAlControlDto = new ExcepcionAlControlDto
                {
                Id = 1,
                TransportistaId = 1,
                MaterialId = 2,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroNombre = "c1"
            };

            var result = target.Crear(new DatosUsuario(),excepcionAlControlDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTransportistas())
                .Returns(new List<TransportistaDto> { new TransportistaDto { RazonSocial = "a" }, new TransportistaDto() });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto(), new CentroDto(), new CentroDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionAlControl>())).Returns(resultado);

            var excepcionAlControlDto = new ExcepcionAlControlDto
                {
                Id = 1,
                TransportistaId = 1,
                MaterialId = 2,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroNombre = "c1"
            };

            var result = target.Crear(new DatosUsuario(), excepcionAlControlDto) as ViewResult;

            List<SelectListItem> centros = target.ViewBag.Centros;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(centros.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarTransportistas())
                .Returns(new List<TransportistaDto> { new TransportistaDto { RazonSocial = "a" }, new TransportistaDto() });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto(), new CentroDto(), new CentroDto() });

            servRepositorioMock.Setup(s => s.ObtenerExcepcionAlControl(It.IsAny<int>()))
                .Returns(new ExcepcionAlControlDto
                    {
                    Id = 1,
                    TransportistaId = 1,
                    MaterialId = 2,
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroNombre = "c1"
                });

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> centros = target.ViewBag.Centros;

            Assert.NotNull(result.Model);
            Assert.That(centros.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionAlControl>()))
                .Returns(new Resultado());

            var excepcionAlControlDto = new ExcepcionAlControlDto
                {
                Id = 1,
                TransportistaId = 1,
                MaterialId = 2,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroNombre = "c1"
            };

            var result = target.Modificar(new DatosUsuario(), excepcionAlControlDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTransportistas())
                .Returns(new List<TransportistaDto> { new TransportistaDto { RazonSocial = "a" }, new TransportistaDto() });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto(), new CentroDto(), new CentroDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionAlControl>())).Returns(resultado);

            var excepcionAlControlDto = new ExcepcionAlControlDto
                {
                Id = 1,
                TransportistaId = 1,
                MaterialId = 2,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroNombre = "c1"
            };

            var result = target.Modificar(new DatosUsuario(), excepcionAlControlDto) as ViewResult;

            List<SelectListItem> centros = target.ViewBag.Centros;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(centros.Count(), Is.EqualTo(3));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionAlControl>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionAlControl>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
