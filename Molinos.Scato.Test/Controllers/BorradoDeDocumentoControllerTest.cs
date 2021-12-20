using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class BorradoDeDocumentoControllerTest
    {
        private BorradoDeDocumentoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IListaDeWorkflows> servWorkflowsMock;
        private List<RecorridoDto> recorridos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servWorkflowsMock = new Mock<IListaDeWorkflows>();
            target = new BorradoDeDocumentoController(
                null, servRepositorioMock.Object, servComandosMock.Object, servWorkflowsMock.Object);

            recorridos = new List<RecorridoDto>
                {
                    new RecorridoDto()
                        {
                            Id = 1,
                            Patente = "AAA111"
                        },
                    new RecorridoDto()
                        {
                            Id = 2,
                            Patente = "BBB222"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RecorridoDto>(recorridos, 1, 2, 2));

            var filter = new FiltroRecorridoModel();
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<RecorridoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Patente, Is.EqualTo("AAA111"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RecorridoDto>(recorridos, 1, 2, 2));

            var filter = new FiltroRecorridoModel();
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<RecorridoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Patente, Is.EqualTo("AAA111"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRecorridosPorDocumentoPatenteYCentro(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>(), "BBB222", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RecorridoDto>(new List<RecorridoDto> { recorridos[1] }, 1, 2, 2));

            var filter = new FiltroRecorridoModel { Patente = "BBB222" };
            var datosUsuario = new DatosUsuario { CentroId = 1 };

            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<RecorridoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Patente, Is.EqualTo("BBB222"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarRecorrido>())).Returns(new Resultado());

            target.EliminarTerminado(new DatosUsuario(), 0, "");

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
        }

    }
}
