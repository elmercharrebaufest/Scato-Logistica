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
    public class ActividadConCargaAutomaticaControllerTest
    {
        private ActividadConCargaAutomaticaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioWorkflows> servWorkflowsMock;
        private List<ActividadConCargaAutomaticaDto> actividades;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servWorkflowsMock = new Mock<IServicioWorkflows>();
            target = new ActividadConCargaAutomaticaController(
                null, servRepositorioMock.Object, servComandosMock.Object, servWorkflowsMock.Object);

            actividades = new List<ActividadConCargaAutomaticaDto>
                {
                    new ActividadConCargaAutomaticaDto
                        {
                            Id = 1,
                            CentroId = 1,
                            Actividad = "Actividad 1",
                            WorkflowId = 1
                        },
                    new ActividadConCargaAutomaticaDto
                        {
                            Id = 2,
                            CentroId = 2,
                            Actividad = "Actividad 2",
                            WorkflowId = 2
                        }
                };
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesConCargaAutomatica(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadConCargaAutomaticaDto>(actividades, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });

            var result = target.Listar(new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<ActividadConCargaAutomaticaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Actividad, Is.EqualTo("Actividad 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoActividadesConCargaAutomatica(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ActividadConCargaAutomaticaDto>(new List<ActividadConCargaAutomaticaDto> { actividades[1] }, 1, 1, 1));
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>()))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });

            var result = target.Listar(new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<ActividadConCargaAutomaticaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Actividad, Is.EqualTo("Actividad 2"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearActividadConCargaAutomatica>()))
                .Returns(new Resultado());

            var result = target.Crear(actividades[0], new DatosUsuario { CentroId = 1 }) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearActividadConCargaAutomatica>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> {new WorkflowDto {Id = 1, Descripcion = "Workflow 1"}});
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> {"Actividad 1"});
            servRepositorioMock.Setup(s => s.ListarPermisosDeActividad()).Returns(new List<string> { "Actividad 1", "Actividad 2" });

            var result = target.Crear(actividades[0], new DatosUsuario { CentroId = 1 }) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerActividadConCargaAutomatica(1)).Returns(actividades[0]);
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "Workflow 1" } });
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> { "Actividad 1", "Actividad 3" });
            servRepositorioMock.Setup(s => s.ListarPermisosDeActividad()).Returns(new List<string> { "Actividad 1", "Actividad 2" });

            var result = target.Modificar(1, new DatosUsuario{CentroId = 1}) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
            IEnumerable<SelectListItem> actividadesWf = target.ViewBag.Actividades;
            Assert.That(actividadesWf.Select(x => x.Value), Is.EquivalentTo(new List<string> { "Actividad 1" }));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarActividadConCargaAutomatica>())).Returns(new Resultado());

            var camaraDto = new ActividadConCargaAutomaticaDto
            {
                Id = 1,
                WorkflowId = 1,
                Actividad = "Actividad 1",
            };

            var result = target.Modificar(camaraDto, new DatosUsuario { CentroId = 1 }) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarActividadConCargaAutomatica>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "Workflow 1" } });
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>()))
                .Returns(new List<string> { "Actividad 1" });
            servRepositorioMock.Setup(s => s.ListarPermisosDeActividad()).Returns(new List<string> { "Actividad 1", "Actividad 2" });

            var camaraDto = new ActividadConCargaAutomaticaDto
            {
                Id = 1,
                WorkflowId = 1,
                Actividad = "Actividad 1",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(camaraDto, new DatosUsuario { CentroId = 1 }) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarActividadConCargaAutomatica>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
