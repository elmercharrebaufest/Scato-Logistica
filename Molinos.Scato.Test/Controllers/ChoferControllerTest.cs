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
    public class ChoferControllerTest
    {
        private ChoferController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ChoferDto> choferes;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ChoferController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            choferes = new List<ChoferDto>
                {
                    new ChoferDto
                        {
                            Id = 1,
                            Apellido = "a",
                            Nombre = "a",
                            NumeroDeDocumento = "22",
                            TipoDocumentoIdentidadId = 1
                        },
                    new ChoferDto
                        {
                            Id = 1,
                            Apellido = "b",
                            Nombre = "b",
                            NumeroDeDocumento = "11",
                            TipoDocumentoIdentidadId = 1
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarChoferes(It.IsAny<string>(),It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ChoferDto>(choferes,1,2,2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<ChoferDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("a"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarChoferes(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ChoferDto>(choferes, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<ChoferDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("a"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto{DescripcionCorta = "c"}, new TipoDocumentoIdentidadDto {DescripcionCorta = "d"} });

            var result = target.Crear() as ViewResult;
            List<SelectListItem> tiposDocumentoIdentidad = target.ViewBag.TiposDocumentoIdentidad;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(tiposDocumentoIdentidad.Count(), Is.EqualTo(3));
            Assert.That(tiposDocumentoIdentidad[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>()))
                .Returns(new Resultado());
            
            var choferDto = new ChoferDto
            {
                Id = 1,
                Apellido = "c",
                Nombre = "c",
                NumeroDeDocumento = "33",
                TipoDocumentoIdentidadId = 1
            };

            var result = target.Crear(choferDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto{DescripcionCorta = "c"}, new TipoDocumentoIdentidadDto{DescripcionCorta = "d"} });

            var resultado = new Resultado();
            resultado.Error("Error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(resultado);

            var choferDto = new ChoferDto
            {
                Id = 1,
                Apellido = "c",
                Nombre = "c",
                NumeroDeDocumento = "33",
                TipoDocumentoIdentidadId = 1
            };

            var result = target.Crear(choferDto, datosUsuario) as ViewResult;

            List<SelectListItem> tiposDocumentoIdentidad = target.ViewBag.TiposDocumentoIdentidad;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(tiposDocumentoIdentidad.Count(), Is.EqualTo(3));
            Assert.That(tiposDocumentoIdentidad[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto{DescripcionCorta = "c"}, new TipoDocumentoIdentidadDto{DescripcionCorta = "d"} });
            servRepositorioMock.Setup(s => s.ObtenerChofer(It.IsAny<int>()))
                .Returns(new ChoferDto
                            {
                                Id = 1,
                                Apellido = "c",
                                Nombre = "c",
                                NumeroDeDocumento = "33",
                                TipoDocumentoIdentidadId = 1
                            } );

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> tiposDocumentoIdentidad = target.ViewBag.TiposDocumentoIdentidad;

            Assert.NotNull(result.Model);
            Assert.That(tiposDocumentoIdentidad.Count(), Is.EqualTo(3));
            Assert.That(tiposDocumentoIdentidad[0].Text, Is.EqualTo("b"));
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>()))
                .Returns(new Resultado());

            var choferDto = new ChoferDto
            {
                Id = 1,
                Apellido = "c",
                Nombre = "c",
                NumeroDeDocumento = "33",
                TipoDocumentoIdentidadId = 1
            };

            var result = target.Modificar(choferDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto{DescripcionCorta = "c"}, new TipoDocumentoIdentidadDto{DescripcionCorta = "d"} });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);

            var choferDto = new ChoferDto
            {
                Id = 1,
                Apellido = "c",
                Nombre = "c",
                NumeroDeDocumento = "33",
                TipoDocumentoIdentidadId = 1
            };

            var result = target.Modificar(choferDto, datosUsuario) as ViewResult;

            List<SelectListItem> tiposDocumentoIdentidad = target.ViewBag.TiposDocumentoIdentidad;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(tiposDocumentoIdentidad.Count(), Is.EqualTo(3));
            Assert.That(tiposDocumentoIdentidad[0].Text, Is.EqualTo("b"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarChofer>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarChofer>())).Returns(resultado);

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
