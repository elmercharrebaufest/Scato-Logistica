using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using Comando = Molinos.Scato.Dominio.Comandos.Comando;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class NirsControllerTest
    {
        private NirsController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> servOrquestadorMock;
        private List<NirsDto> Nirss;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servOrquestadorMock = new Mock<IServicioOrquestador>();
            target = new NirsController(
                null, servRepositorioMock.Object, servComandosMock.Object, servOrquestadorMock.Object);

            Nirss = new List<NirsDto>
                {
                    new NirsDto
                        {
                            Id = 1,
                            Descripcion = "Nirs 1",
                            CentroId = 1,
                            Codigo = "1",
                            DescripcionCorta = "H1",
                            Modalidad = Modalidad.Manual
                        },
                    new NirsDto
                        {
                            Id = 2,
                            Descripcion = "Nirs 2",
                            CentroId = 2,
                            Codigo = "2",
                            DescripcionCorta = "H2",
                            Modalidad = Modalidad.Automática
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoNirs(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<NirsDto>(Nirss, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<NirsDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Nirs 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoNirs(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<NirsDto>(Nirss, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<NirsDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Nirs 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoNirs("2", It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<NirsDto>(new List<NirsDto> { Nirss[1] }, 1, 1, 1));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "2";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<NirsDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Nirs 2"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearNirs>()))
                .Returns(new Resultado());

            var result = target.Crear(Nirss[0], new DatosUsuario()) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearNirs>())).Returns(resultado);
            servOrquestadorMock.Setup(s => s.ListarNirs()).Returns(new DispositivoDto[] { new DispositivoDto() });

            var result = target.Crear(Nirss[0], new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerNirs(1))
                .Returns(Nirss[0]);
            servOrquestadorMock.Setup(s => s.ListarNirs()).Returns(new DispositivoDto[] { new DispositivoDto() });

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarNirs>()))
                .Returns(new Resultado());

            var NirsDto = new NirsDto
            {
                Id = 1,
                Descripcion = "Nirs modificado",
            };

            var result = target.Modificar(NirsDto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarNirs>()))
                .Returns(new Resultado());
            servOrquestadorMock.Setup(s => s.ListarNirs()).Returns(new DispositivoDto[] { new DispositivoDto() });

            var NirsDto = new NirsDto
            {
                Id = 1,
                Descripcion = "Nirs modificado",
            };

            target.ModelState.AddModelError("", "Error");

            var result = target.Modificar(NirsDto, new DatosUsuario()) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestEliminar()
        {
            var datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarNirs>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }
    }
}
