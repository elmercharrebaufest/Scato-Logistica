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
    public class TarjetaSupervisorControllerTest
    {
        private TarjetaSupervisorController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TarjetaSupervisorDto> tarjetaSupervisor;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TarjetaSupervisorController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            tarjetaSupervisor = new List<TarjetaSupervisorDto>
                {
                    new TarjetaSupervisorDto
                        {
                            Id = 1,
                            Descripcion = "Tarjeta 1",
                            Salida = 1,
                            CentroId = 1,
                        },
                    new TarjetaSupervisorDto
                        {
                            Id = 1,
                            Descripcion = "Tarjeta 2",
                            Salida = 1,
                            CentroId = 1,
                        },
                };
            servRepositorioMock.Setup(s => s.ListarPaginadoTarjetasSupervisor(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>())).Returns(new ListaPaginada<TarjetaSupervisorDto>(tarjetaSupervisor, 1, 2, 2));
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario { CentroId = 1 }, null) as ViewResult;
            IEnumerable<TarjetaSupervisorDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { tarjetaSupervisor[0].Id, tarjetaSupervisor[1].Id }));
        }

        [Test]
        public void TestListar()
        {
            var result = target.Listar(new DatosUsuario { CentroId = 1 }, null) as ViewResult;
            IEnumerable<TarjetaSupervisorDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { tarjetaSupervisor[0].Id, tarjetaSupervisor[1].Id }));
        }

        [Test]
        public void TestCrear()
        {
             servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajo())
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { NombrePuesto = "b" }, new PuestoDeTrabajoDto{NombrePuesto = "c"}, new PuestoDeTrabajoDto { NombrePuesto = "d"} });

            var result = target.Crear(new DatosUsuario()) as ViewResult;
            List<SelectListItem> puestosDeTrabajo = target.ViewBag.PuestosDeTrabajo;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(puestosDeTrabajo.Count(), Is.EqualTo(3));
            Assert.That(puestosDeTrabajo[0].Text, Is.EqualTo("b"));

        }


        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTarjetaSupervisor>())).Returns(new Resultado());
            var result = target.Crear(tarjetaSupervisor[0], "[]", new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearTarjetaSupervisor>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajo())
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { NombrePuesto = "b" }, new PuestoDeTrabajoDto { NombrePuesto = "c" }, new PuestoDeTrabajoDto { NombrePuesto = "d" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTarjetaSupervisor>())).Returns(resultado);

            var result = target.Crear(new DatosUsuario()) as ViewResult;

            List<SelectListItem> puestosDeTrabajo = target.ViewBag.PuestosDeTrabajo;
            
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(puestosDeTrabajo.Count(), Is.EqualTo(3));
            Assert.That(puestosDeTrabajo[0].Text, Is.EqualTo("b"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTarjetaSupervisor>())).Returns(new Resultado());

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
