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
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class TipoComercialPorWfControllerTest
    {
        private TipoComercialPorWfController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TipoComercialPorWfDto> tiposComercialesPorWf;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TipoComercialPorWfController(null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            tiposComercialesPorWf = new List<TipoComercialPorWfDto>{new TipoComercialPorWfDto{WorkflowId = 1, TipoComercialId = 1, WorkflowDescripcion = "W1", TipoComercialDescripcion = "T1"}, 
                                                                        new TipoComercialPorWfDto{WorkflowId = 2, TipoComercialId = 2, WorkflowDescripcion = "W2", TipoComercialDescripcion = "T2"}};
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWf(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TipoComercialPorWfDto>(tiposComercialesPorWf, 1, 2, 2));

            var result = target.Index(new DatosUsuario { CentroId = 1 }, It.IsAny<string>()) as ViewResult;
            IEnumerable<TipoComercialPorWfDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.WorkflowId), Is.EquivalentTo(new List<int> { 1,2}));
            Assert.That(results.Select(x => x.WorkflowDescripcion), Is.EquivalentTo(new List<string> { "W1", "W2" }));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWf(It.IsAny<string>(),It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<TipoComercialPorWfDto>(tiposComercialesPorWf, 1, 2, 2));

            var result = target.Listar(new DatosUsuario { CentroId = 1 }, It.IsAny<string>()) as ViewResult;
            IEnumerable<TipoComercialPorWfDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.WorkflowId), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That(results.Select(x => x.WorkflowDescripcion), Is.EquivalentTo(new List<string> { "W1", "W2" }));
        }

        [Test]
        public void TestCrear()
        {
            var workflows = new List<WorkflowDto>
                {
                    new WorkflowDto {Id = 1, Descripcion = "W1", Activo = true},
                    new WorkflowDto {Id = 2, Descripcion = "W2", Activo = false}
                };

            servRepositorioMock.Setup(s => s.ListarWorkflowsPorCentro(It.IsAny<int>())).Returns(workflows);
            var result = target.Crear( new DatosUsuario{CentroId = 1}) as ViewResult;
            List<SelectListItem> results = target.ViewBag.Worflows;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(results.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTipoComercialPorWf>()))
                .Returns(new Resultado());

            var result = target.Crear(new TipoComercialPorWfDto(), datosUsuario) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTipoComercialPorWf>())).Returns(resultado);

            var result = target.Crear(new TipoComercialPorWfDto(), datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTipoComercialPorWf>())).Returns(new Resultado());

            var actual = target.Eliminar(1, 1, datosUsuario) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTipoComercialPorWf>())).Returns(resultado);

            var actual = target.Eliminar(1, 1, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }

        [Test]
        public void TestObtenerTiposComerciales()
        {
            var tiposViejos = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "T1"},
                    new TipoComercialDto {Id = 2, Descripcion = "T2"}
                };
            servRepositorioMock.Setup(s => s.ObtenerWorkflow(It.IsAny<int>()))
                               .Returns(new WorkflowDto { Id = 1, TiposComercialesAsociados = tiposViejos });

            var tiposNuevos = new List<TipoComercialDto>(tiposViejos)
                {
                    new TipoComercialDto {Id = 3, Descripcion = "T3"}
                };

            servRepositorioMock.Setup(s => s.ListarTiposComerciales()).Returns(tiposNuevos);

            var resultado = target.ObtenerTiposComerciales(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo(@"[{""value"":1,""text"":""T1"",""disable"":true},{""value"":2,""text"":""T2"",""disable"":true},{""value"":3,""text"":""T3"",""disable"":false}]"));


        }

    }
}
