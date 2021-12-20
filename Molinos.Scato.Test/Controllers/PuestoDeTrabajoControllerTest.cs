using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
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
    public class PuestoDeTrabajoControllerTest
    {
        private PuestoDeTrabajoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioEstadoPuesto> estado;

        private List<PuestoDeTrabajoDto> puestos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            estado = new Mock<IServicioEstadoPuesto>();
            target = new PuestoDeTrabajoController(
                null, servRepositorioMock.Object, servComandosMock.Object, orquestadorMock.Object, null, estado.Object);

            puestos = new List<PuestoDeTrabajoDto>
                {
                    new PuestoDeTrabajoDto
                        {
                            Id = 1,
                            CentroId = 1,
                            NombrePuesto = "Puesto 1",
                            NombrePc = "PC 1",
                            Entrada = "Barrera 1",
                            Lector = "Lector 1",
                        },
                    new PuestoDeTrabajoDto
                        {
                            Id = 2,
                            CentroId = 2,
                            NombrePuesto = "Puesto 2",
                            NombrePc = "PC 2",
                            Entrada = "Barrera 2",
                            Lector = "Lector 2",
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(puestos, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(puestos, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoPuestosDeTrabajo("2", It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestoDeTrabajoDto>(new List<PuestoDeTrabajoDto> { puestos[1] }, 1, 1, 1));

            const string filter = "2";
            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<PuestoDeTrabajoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].NombrePuesto, Is.EqualTo("Puesto 2"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", puestos[0],"[]") as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>())).Returns(resultado);
            orquestadorMock.Setup(s => s.ListarLectores()).Returns(new[]{ new DispositivoDto()});
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new[] { new DispositivoDto() });
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzas(It.IsAny<int>())).Returns(new List<BalanzaDto>());

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", puestos[0],"[]") as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestCrearPostBarrerasVacio()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestoDeTrabajo>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzas(It.IsAny<int>())).Returns(new List<BalanzaDto>());
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, "", "", "","", puestos[0],"") as ContentResult;
            
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.Null(result);
            Assert.False(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(1))
                .Returns(puestos[0]);
            orquestadorMock.Setup(s => s.ListarLectores()).Returns(new[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new[] { new DispositivoDto() });
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzas(It.IsAny<int>())).Returns(new List<BalanzaDto>());

            var result = target.Modificar(1,new DatosUsuario()) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            var result = target.Modificar(camaraDto, "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[{\"Codigo\":\"Barem01\",\"Descripcion\":\"Bar1\"}]", "[]", "[]", new DatosUsuario(),"[]") as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        [Test]
        public void TestModificarPostBarrerasVacio()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzas(It.IsAny<int>())).Returns(new List<BalanzaDto>());

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            var result = target.Modificar(camaraDto, "", "", "", "", new DatosUsuario(),"") as ContentResult;
            
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.Null(result);
            Assert.False(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestoDeTrabajo>()))
                .Returns(new Resultado());
            orquestadorMock.Setup(s => s.ListarLectores()).Returns(new DispositivoDto[] { new DispositivoDto() });
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new DispositivoDto[] { new DispositivoDto() });
            servRepositorioMock.Setup(x => x.ListarTodasLasBalanzas(It.IsAny<int>())).Returns(new List<BalanzaDto>());

            var camaraDto = new PuestoDeTrabajoDto
            {
                Id = 1,
                NombrePuesto = "Nombre modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(camaraDto, "", "", "", "", new DatosUsuario(),"") as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificarModalidad()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(1))
                .Returns(puestos[0]);

            var result = target.ModificarModalidad(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarModalidadPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarModalidadPuestoDeTrabajo>()))
                .Returns(new Resultado());

            var datosUsuario = new DatosUsuario
                {
                    NombreUsuario = "diego"
                };

            var cambioModel = new CambioModalidadPuestoModel
            {
                Id = 1,
                Automatico = true,
                Motivo = "Por que si"
            };

            var result = target.ModificarModalidad(cambioModel, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.Is<ModificarModalidadPuestoDeTrabajo>(c => c.IdPuesto == 1 && c.Automatico == cambioModel.Automatico && c.Usuario == datosUsuario.NombreUsuario)), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarPuestoDeTrabajo>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }


    }
}
