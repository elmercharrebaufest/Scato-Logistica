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
    class TalonarioControllerTest
    {
        private TalonarioController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TalonarioDto> talonarios;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TalonarioController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            talonarios = new List<TalonarioDto>
                {
                    new TalonarioDto
                        {
                            Id = 1,
                            Descripcion = "talonario 1"
                        },
                    new TalonarioDto
                        {
                            Id = 2,
                            Descripcion = "talonario 2"
                        }
                };

        }





        [Test]
        public void TestIndex()
        {

            servRepositorioMock.Setup(s => s.ListarPaginadoTalonario(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TalonarioDto>(talonarios, 1, 2, 2));
            

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<TalonarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("talonario 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTalonario(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TalonarioDto>(talonarios, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<TalonarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("talonario 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTalonario("2", It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TalonarioDto>(new List<TalonarioDto> { talonarios[1] }, 1, 1, 1));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "2";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<TalonarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("talonario 2"));
        }

        [Test]
        public void TestCrear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            var result = target.Crear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTalonario>()))
                .Returns(new Resultado());

            var result = target.Crear(talonarios[0]) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTalonario>())).Returns(resultado);

            var result = target.Crear(talonarios[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerTalonario(1))
                .Returns(talonarios[0]);

            var result = target.Modificar(1) as ViewResult;


            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTalonario>()))
                .Returns(new Resultado());

            var talonarioDto = new TalonarioDto
            {
                Id = 1,
                Descripcion = "Talonario modificado",
            };

            var result = target.Modificar(talonarioDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTalonario>()))
                .Returns(new Resultado());

            var talonarioDto = new TalonarioDto
            {
                Id = 1,
                Descripcion = "Talonario modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(talonarioDto) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTalonario>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
