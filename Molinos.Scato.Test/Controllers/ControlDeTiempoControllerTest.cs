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
    public class ControlDeTiempoControllerTest
    {
        private ControlDeTiempoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioWorkflows> servWorkflowsMock;
        private List<ControlDeTiempoDto> controlesDeTiempo;
        private List<WorkflowDto> workflows;


        
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servWorkflowsMock = new Mock<IServicioWorkflows>();
            target = new ControlDeTiempoController(
                null, servRepositorioMock.Object, servComandosMock.Object, servWorkflowsMock.Object);

            controlesDeTiempo = new List<ControlDeTiempoDto>
                {
                    new ControlDeTiempoDto
                        {
                            Id = 1,
                            ActividadDesde = "A1",
                            ActividadHasta = "A2",
                            WorkflowCodigo = "wc",
                            TiempoMaximo = 4,
                            CodigoControl = "111",
                        },
                    new ControlDeTiempoDto
                        {
                            Id = 2,
                            ActividadDesde = "A1",
                            ActividadHasta = "A2",
                            WorkflowCodigo = "wc",
                            TiempoMaximo = 4,
                            CodigoControl = "222",
                        },
                };

            workflows = new List<WorkflowDto>
                {
                    new WorkflowDto
                        {
                            Id = 1,
                            Descripcion = "Desc1"
                        },
                    new WorkflowDto
                        {
                            Id = 2,
                            Descripcion = "Desc2"
                        },
                };

            servRepositorioMock.Setup(s => s.ListarPaginadoControlesDeTiempo(It.IsAny<Paginacion>(), It.IsAny<int>())).Returns(new ListaPaginada<ControlDeTiempoDto>(controlesDeTiempo, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>())).Returns(workflows);
            servWorkflowsMock.Setup(s => s.ListarActividadesPorDefinicionWorkflow(It.IsAny<int>())).Returns(new List<string>());
            

        }


        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<ControlDeTiempoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>{1, 2}));
        }


        [Test]
        public void TestListar()
        {
            var result = target.Listar(new DatosUsuario()) as ViewResult;
            IEnumerable<ControlDeTiempoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
        }

        [Test]
        public void TestCrear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            target.Crear(datosUsuario);
            IEnumerable<SelectListItem> results = target.ViewBag.Workflows;
            Assert.That(results.Select(x => x.Value), Is.EquivalentTo(new List<string> { "1", "2" }));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearControlDeTiempo>())).Returns(new Resultado());
            var result = target.Crear(controlesDeTiempo[0], new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearControlDeTiempo>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearControlDeTiempo>())).Returns(resultado);
            var result = target.Crear(controlesDeTiempo[0], new DatosUsuario()) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerControlDeTiempo(It.IsAny<int>())).Returns(controlesDeTiempo[0]);
            var result = target.Modificar(new DatosUsuario(), It.IsAny<int>()) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(result.Model, Is.EqualTo(controlesDeTiempo[0]));
        }


        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarControlDeTiempo>())).Returns(new Resultado());
            var result = target.Modificar(controlesDeTiempo[0], new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ModificarControlDeTiempo>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarControlDeTiempo>())).Returns(resultado);
            var result = target.Modificar(controlesDeTiempo[0], new DatosUsuario()) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
            Assert.That(((IEnumerable<SelectListItem>)result.ViewBag.Workflows).Select(s => s.Value), Is.EquivalentTo(new List<string> { "1", "2" }));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarControlDeTiempo>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
