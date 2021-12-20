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
    public class RolControllerTest
    {
        private RolController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<RolDto> roles;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new RolController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            roles = new List<RolDto>
                {
                    new RolDto
                        {
                            Id = 1,
                            Descripcion = "Rol 1"
                        },
                    new RolDto
                        {
                            Id = 2,
                            Descripcion = "Rol 2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRoles(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RolDto>(roles, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<RolDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Rol 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRoles(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RolDto>(roles, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<RolDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Rol 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRoles("2", It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RolDto>(new List<RolDto> { roles[1] }, 1, 1, 1));

            const string filter = "2";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<RolDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Rol 2"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto>{new PermisoDto {Id = 1, Descripcion = "Permiso 1"},new PermisoDto {Id = 2, Descripcion = "Permiso 2"}});

            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRol>()))
                .Returns(new Resultado());

            const string json = "[{\"Id\":\"1\",\"TipoPermiso\":\"0\",\"TipoPermisoDesc\":\"Abm\",\"Descripcion\":\"Abm Tipo Documento Identidad\"}]";
            var result = target.Crear(roles[0], json) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            const string json = "[{\"Id\":\"1\",\"TipoPermiso\":\"0\",\"TipoPermisoDesc\":\"Abm\",\"Descripcion\":\"Abm Tipo Documento Identidad\"}]";
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRol>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var result = target.Crear(roles[0], json) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            servRepositorioMock.Setup(s => s.ObtenerRol(1)).Returns(roles[0]);

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRol>()))
                .Returns(new Resultado());

            var rolDto = new RolDto
            {
                Id = 1,
                Descripcion = "Rol modificado",
            };

            const string json = "[{\"Id\":\"1\",\"TipoPermiso\":\"0\",\"TipoPermisoDesc\":\"Abm\",\"Descripcion\":\"Abm Tipo Documento Identidad\"}]";
            var result = target.Modificar(rolDto, json) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRol>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarPermisos())
                .Returns(new List<PermisoDto> { new PermisoDto { Id = 1, Descripcion = "Permiso 1" }, new PermisoDto { Id = 2, Descripcion = "Permiso 2" } });

            var rolDto = new RolDto
            {
                Id = 1,
                Descripcion = "Rol modificado",
            };

            var json = "[]";
            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(rolDto, json) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarRol>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
