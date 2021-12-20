using System;
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
    public class InhabilitacionChoferControllerTest
    {
        private InhabilitacionChoferController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<InhabilitacionChoferDto> inthabilitaciones;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new InhabilitacionChoferController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            inthabilitaciones = new List<InhabilitacionChoferDto>
                {
                    new InhabilitacionChoferDto
                        {
                            Id = 1,
                            TipoDocumentoIdentidadId = 1,
                            NumeroDeDocumento = "123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                    new InhabilitacionChoferDto
                        {
                            Id = 2,
                            TipoDocumentoIdentidadId = 1,
                            NumeroDeDocumento = "1234",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarInhabilitacionChoferes(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<InhabilitacionChoferDto>(inthabilitaciones, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<InhabilitacionChoferDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarInhabilitacionChoferes(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<InhabilitacionChoferDto>(inthabilitaciones, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.ListarDatos(datosUsuario, filter) as ViewResult;
            IEnumerable<InhabilitacionChoferDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto(), new TipoDocumentoIdentidadDto() });

            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            List<SelectListItem> tiposDocumentoIdentidad = target.ViewBag.TiposDocumentoIdentidad;

            Assert.That(tiposDocumentoIdentidad.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestCrearPost()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto(), new TipoDocumentoIdentidadDto() });

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearInhabilitacionChofer>()))
                .Returns(new Resultado());
            const string json = "[]";
            var inhabilitacionChoferDto = new InhabilitacionChoferDto
            {
                Id = 2,
                TipoDocumentoIdentidadId = 1,
                NumeroDeDocumento = "1234",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                Motivo = "m",
                Adjuntos = new List <AdjuntoDto>()
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, json,inhabilitacionChoferDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { DescripcionCorta = "b" }, new TipoDocumentoIdentidadDto(), new TipoDocumentoIdentidadDto() });
            servRepositorioMock.Setup(s => s.ListarAdjuntosChofer(2))
                 .Returns(new List<AdjuntoDto> { new AdjuntoDto { Archivo = "archivo" }, new AdjuntoDto(), new AdjuntoDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearInhabilitacionChofer>())).Returns(resultado);

            var inhabilitacionChoferDto = new InhabilitacionChoferDto
            {
                Id = 2,
                TipoDocumentoIdentidadId = 1,
                NumeroDeDocumento = "1234",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                Motivo = "m",
                Adjuntos = new List <AdjuntoDto>()
                
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string json = "[]";


            var result = target.Crear(datosUsuario, json,inhabilitacionChoferDto) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarInhabilitacionChofer>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarInhabilitacionChofer>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
