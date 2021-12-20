using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PermisoControllerTest
    {
        private PermisoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<PermisoDto> permisos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new PermisoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            permisos = new List<PermisoDto>
                {
                    new PermisoDto
                        {
                            Id = 1,
                            Descripcion = "Permiso 1",
                            Codigo = PermisosScato.AbmAlmacen
                        },
                    new PermisoDto
                        {
                            Id = 2,
                            Descripcion = "Permiso 2",
                            Codigo = PermisosScato.AbmBalanza
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPermisos(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PermisoDto>(permisos, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<PermisoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Permiso 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPermisos(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PermisoDto>(permisos, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<PermisoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Permiso 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPermisos("2", It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PermisoDto>(new List<PermisoDto> { permisos[1] }, 1, 1, 1));

            const string filter = "2";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<PermisoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Permiso 2"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPermiso>()))
                .Returns(new Resultado());

            var result = target.Crear(permisos[0]) as ContentResult;
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPermiso>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var result = target.Crear(permisos[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerPermiso(1))
                .Returns(permisos[0]);

            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPermiso>()))
                .Returns(new Resultado());

            var permisoDto = new PermisoDto
            {
                Id = 1,
                Descripcion = "Permiso modificado",
                Codigo = PermisosScato.AbmAlmacen
            };

            var result = target.Modificar(permisoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPermiso>()))
                .Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var permisoDto = new PermisoDto
            {
                Id = 1,
                Descripcion = "Permiso modificado",
                Codigo = PermisosScato.AbmAlmacen
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(permisoDto) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarPermiso>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
