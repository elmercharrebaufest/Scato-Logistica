using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
    public class ActividadPorDispositivoControllerTest
    {
        private ActividadPorDispositivoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioWorkflows> servWorkflowsMock;
        private List<ActividadPorDispositivoDto> actividades;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            servWorkflowsMock = new Mock<IServicioWorkflows>();
            target = new ActividadPorDispositivoController(
                null, servRepositorioMock.Object, servComandosMock.Object, orquestadorMock.Object, servWorkflowsMock.Object);

            actividades = new List<ActividadPorDispositivoDto>
                {
                    new ActividadPorDispositivoDto
                        {
                            Id = 1,
                            CentroId = 1,
                            PuestoDeTrabajoId = 1,
                            Actividad = "Actividad 1",
                            Salida = "Salida 1",
                            WorkflowId = 1
                        },
                    new ActividadPorDispositivoDto
                        {
                            Id = 2,
                            CentroId = 2,
                            PuestoDeTrabajoId = 2,
                            Actividad = "Actividad 2",
                            Salida = "Salida 2",
                            WorkflowId = 2
                        }
                };
        }

        [Test]
        public void TestIndexPuestoEnSesion()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesPorBarreraSemaforo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadPorDispositivoDto>(actividades, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> {new PuestoDeTrabajoDto {Id = 1, NombrePuesto = "Puesto 1"}});

            var result = target.Index(new DatosUsuario(){PuestoDeTrabajoId = 5}) as ViewResult;
            IEnumerable<ActividadPorDispositivoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { }));
            Assert.That(((ActividadPorDispositivoDto)result.Model).PuestoDeTrabajoId, Is.EqualTo(5));
        }

        [Test]
        public void TestIndexPrimerPuesto()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesPorBarreraSemaforo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadPorDispositivoDto>(actividades, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });

            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<ActividadPorDispositivoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { }));
            Assert.That(((ActividadPorDispositivoDto)result.Model).PuestoDeTrabajoId, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesPorBarreraSemaforo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadPorDispositivoDto>(actividades, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });

            var result = target.Listar(1, new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<ActividadPorDispositivoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Actividad, Is.EqualTo("Actividad 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesPorBarreraSemaforo(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadPorDispositivoDto>(new List<ActividadPorDispositivoDto> { actividades[1] }, 1, 1, 1));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });

            var result = target.Listar(1, new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<ActividadPorDispositivoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Actividad, Is.EqualTo("Actividad 2"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearActividadPorDispositivo>()))
                .Returns(new Resultado());

            var result = target.Crear(actividades[0], "[]", "[]", new DatosUsuario { CentroId = 1 },"[]") as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearActividadPorDispositivo>())).Returns(resultado);
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new DispositivoDto[] { new DispositivoDto() });
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> {new WorkflowDto {Id = 1, Descripcion = "Workflow 1"}});
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> {"Actividad 1"});

            var result = target.Crear(actividades[0], "[]", "[]", new DatosUsuario { CentroId = 1 }, "[]") as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerActividadPorDispositivo(1)).Returns(actividades[0]);
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new DispositivoDto[] { new DispositivoDto() });
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "Workflow 1" } });
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> { "Actividad 1" });

            var result = target.Modificar(1, new DatosUsuario{CentroId = 1}) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarActividadPorDispositivo>())).Returns(new Resultado());

            var camaraDto = new ActividadPorDispositivoDto
            {
                Id = 1,
                PuestoDeTrabajoId = 1,
                WorkflowId = 1,
                Actividad = "Actividad 1",
                Salida = "Salida cambio"
            };

            var result = target.Modificar(camaraDto, "[]", "[]", new DatosUsuario { CentroId = 1 }, "[]") as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarActividadPorDispositivo>())).Returns(new Resultado());
            orquestadorMock.Setup(s => s.ListarBarrerasSemaforos()).Returns(new DispositivoDto[] { new DispositivoDto() });
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "Workflow 1" } });
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> { "Actividad 1" });

            var camaraDto = new ActividadPorDispositivoDto
            {
                Id = 1,
                PuestoDeTrabajoId = 1,
                WorkflowId = 1,
                Actividad = "Actividad 1",
                Salida = "Salida cambio"
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(camaraDto, "[]", "[]", new DatosUsuario { CentroId = 1 }, "[]") as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarActividadPorDispositivo>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
