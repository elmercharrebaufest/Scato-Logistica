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
    public class TipoDocumentoIdentidadControllerTest
    {
        private TipoDocumentoIdentidadController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TipoDocumentoIdentidadDto> tipos;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TipoDocumentoIdentidadController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            tipos = new List<TipoDocumentoIdentidadDto>
                {
                    new TipoDocumentoIdentidadDto
                        {
                            Id = 1,
                            Descripcion = "Documento Nacional de Identidad",
                            DescripcionCorta = "DNI"
                        },
                    new TipoDocumentoIdentidadDto
                        {
                            Id = 2,
                            Descripcion = "Libreta Cívica",
                            DescripcionCorta = "LC"
                        },
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTiposDocumentoIdentidad(It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TipoDocumentoIdentidadDto>(tipos, 1, 2, 2));

            var result = target.Index() as ViewResult;
            IEnumerable<TipoDocumentoIdentidadDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].DescripcionCorta, Is.EqualTo("DNI"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTiposDocumentoIdentidad(It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TipoDocumentoIdentidadDto>(tipos, 1, 2, 2));

            var result = target.Listar() as ViewResult;
            IEnumerable<TipoDocumentoIdentidadDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].DescripcionCorta, Is.EqualTo("DNI"));
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTipoDocumentoIdentidad>()))
                .Returns(new Resultado());

            var result = target.Crear(tipos[0]) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTipoDocumentoIdentidad>())).Returns(resultado);

            var result = target.Crear(tipos[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerTipoDocumentoIdentidad(It.IsAny<int>()))
                .Returns(new TipoDocumentoIdentidadDto
                    {
                                Id = 1,
                                Descripcion = "Libreta",
                                DescripcionCorta = "L"
                            });

            var result = target.Modificar(1) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTipoDocumentoIdentidad>()))
                .Returns(new Resultado());

            var tipoDto = new TipoDocumentoIdentidadDto
                {
                Id = 1,
                Descripcion = "Libreta",
                DescripcionCorta = "L"
            };

            var result = target.Modificar(tipoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTipoDocumentoIdentidad>())).Returns(resultado);

            var tipoDto = new TipoDocumentoIdentidadDto
                {
                Id = 1,
                Descripcion = "Libreta",
                DescripcionCorta = "L"
            };

            var result = target.Modificar(tipoDto) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTipoDocumentoIdentidad>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTipoDocumentoIdentidad>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
