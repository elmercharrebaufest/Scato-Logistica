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
    public class MaterialPorWorkflowControllerTest
    {
        private MaterialPorWorkflowController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<MaterialPorWorkflowDto> materialesPorWorkflow;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new MaterialPorWorkflowController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            materialesPorWorkflow = new List<MaterialPorWorkflowDto>
                {
                    new MaterialPorWorkflowDto
                        {
                            Id = 1,
                            CentroId = 1,
                            MaterialId = 11,
                            WorkflowId = 1
                        },
                    new MaterialPorWorkflowDto
                        {
                            Id = 2,
                            CentroId = 1,
                            MaterialId = 11,
                            WorkflowId = 2
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMaterialPorWorkflow(It.IsAny<string>(),It.IsAny<Paginacion>(), 1))
                .Returns(new ListaPaginada<MaterialPorWorkflowDto>(materialesPorWorkflow, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index(datosUsuario, "filtro", 1, "Id", DirOrden.Asc) as ViewResult;
            IEnumerable<MaterialPorWorkflowDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].WorkflowId, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMaterialPorWorkflow(It.IsAny<string>(), It.IsAny<Paginacion>(), 1))
                .Returns(new ListaPaginada<MaterialPorWorkflowDto>(materialesPorWorkflow, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Listar(datosUsuario, "filtro", 1, "Id", DirOrden.Asc) as ViewResult;
            IEnumerable<MaterialPorWorkflowDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].WorkflowId, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "b", Activo = true }, new WorkflowDto { Id = 2, Descripcion = "c", Activo = true }, new WorkflowDto { Id = 3, Descripcion = "d", Activo = false } });
            DatosUsuario datosUsuario = new DatosUsuario(){CentroId = 1};

            var result = target.Crear(datosUsuario) as ViewResult;
            List<SelectListItem> workflows = target.ViewBag.Workflows;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(workflows.Count(), Is.EqualTo(2));
            Assert.That(workflows[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMaterialPorWorkflow>()))
                .Returns(new Resultado());

            var excepcionEnvioCamaraDto = new MaterialPorWorkflowDto()
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 11,
                WorkflowId = 1
            };

            var result = target.Crear(datosUsuario, excepcionEnvioCamaraDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "b", Activo = true }, new WorkflowDto { Id = 2, Descripcion = "c", Activo = true }, new WorkflowDto { Id = 3, Descripcion = "d", Activo = false } });
          
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMaterialPorWorkflow>())).Returns(resultado);

            var materialPorWorkflow = new MaterialPorWorkflowDto
            {
                Id = 1,
                CentroId = 5,
                MaterialId = 12,
                WorkflowId = 5
            };

            var result = target.Crear(new DatosUsuario { CentroId = 1 }, materialPorWorkflow) as ViewResult;

            List<SelectListItem> workflows = target.ViewBag.Workflows;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(workflows.Count(), Is.EqualTo(2));
            Assert.That(workflows[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "b" }, new WorkflowDto { Id = 2, Descripcion = "c" }, new WorkflowDto { Id = 3, Descripcion = "d" } });
          
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorWorkflow(It.IsAny<int>()))
                .Returns(new MaterialPorWorkflowDto
                {
                    Id = 1,
                    CentroId = 1,
                    MaterialId = 11,
                    WorkflowId = 1
                });

            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, 1) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMaterialPorWorkflow>()))
                .Returns(new Resultado());

            var tipoDto = new MaterialPorWorkflowDto()
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 11,
                WorkflowId = 2
            };

            var result = target.Modificar(datosUsuario, tipoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                .Returns(new List<WorkflowDto> { new WorkflowDto { Id = 1, Descripcion = "b" }, new WorkflowDto { Id = 2, Descripcion = "c" }, new WorkflowDto { Id = 3, Descripcion = "d" } });
          
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMaterialPorWorkflow>())).Returns(resultado);

            var tipoDto = new MaterialPorWorkflowDto
            {
                Id = 1,
                CentroId = 3,
                MaterialId = 3,
                WorkflowId = 5
            };

            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, tipoDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarMaterialPorWorkflow>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarMaterialPorWorkflow>())).Returns(resultado);

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
