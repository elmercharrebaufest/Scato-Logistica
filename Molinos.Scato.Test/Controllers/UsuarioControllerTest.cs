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
    public class UsuarioControllerTest
    {
        private UsuarioController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<UsuarioDto> usuarios;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new UsuarioController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            usuarios = new List<UsuarioDto>
                {
                    new UsuarioDto
                        {
                            Id = 1,
                            NombreUsuario = "Usuario 1"
                        },
                    new UsuarioDto
                        {
                            Id = 2,
                            NombreUsuario = "Usuario 2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoUsuarios(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<UsuarioDto>(usuarios, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<UsuarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombreUsuario, Is.EqualTo("Usuario 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoUsuarios(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<UsuarioDto>(usuarios, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<UsuarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombreUsuario, Is.EqualTo("Usuario 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoUsuarios("2", It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<UsuarioDto>(new List<UsuarioDto> { usuarios[1] }, 1, 1, 1));

            const string filter = "2";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<UsuarioDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].NombreUsuario, Is.EqualTo("Usuario 2"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarRoles())
                .Returns(new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });

            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearUsuario>()))
                .Returns(new Resultado());

            const string json = "[]";
            var result = target.Crear(usuarios[0], json, json, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            const string json = "[]";
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearUsuario>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarRoles())
                .Returns(new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });

            var result = target.Crear(usuarios[0], json, json, datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarRoles())
                .Returns(new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });

            servRepositorioMock.Setup(s => s.ObtenerUsuario(1)).Returns(usuarios[0]);

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarUsuario>()))
                .Returns(new Resultado());

            var usuarioDto = new UsuarioDto
            {
                Id = 1,
                NombreUsuario = "Usuario modificado",
            };

            var json = "[]";
            var result = target.Modificar(usuarioDto, json, json, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarUsuario>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarRoles())
                .Returns(new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });

            var usuarioDto = new UsuarioDto
            {
                Id = 1,
                NombreUsuario = "Usuario modificado",
            };

            var json = "[]";
            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(usuarioDto, json, json, datosUsuario) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarUsuario>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        [Test]
        public void TestCopiar()
        {
            servRepositorioMock.Setup(s => s.ListarRoles())
                .Returns(new List<RolDto> { new RolDto { Id = 1, Descripcion = "Rol 1" }, new RolDto { Id = 2, Descripcion = "Rol 2" } });
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });

            servRepositorioMock.Setup(s => s.ObtenerUsuario(1)).Returns(usuarios[0]);

            var result = target.Crear(1) as ViewResult;
            var model = (UsuarioDto)result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(model.Id != 0);
            Assert.That(model.Apellido, Is.Empty) ;
            Assert.That(model.Nombre, Is.Empty);
            Assert.That(model.NombreUsuario, Is.Empty);
            Assert.That(model.Email, Is.Empty);
     
        }

    }
}
