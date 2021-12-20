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
    public class InhabilitacionCamionControllerTest
    {
        private InhabilitacionCamionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<InhabilitacionCamionDto> inthabilitaciones;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new InhabilitacionCamionController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            inthabilitaciones = new List<InhabilitacionCamionDto>
                {
                    new InhabilitacionCamionDto
                        {
                            Id = 1,
                            Patente = "AAA123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        },
                    new InhabilitacionCamionDto
                        {
                            Id = 2,
                            Patente = "AAA123",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            Motivo = "m"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarInhabilitacionCamiones(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<InhabilitacionCamionDto>(inthabilitaciones, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<InhabilitacionCamionDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarInhabilitacionCamiones(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<InhabilitacionCamionDto>(inthabilitaciones, 1, 2, 2));

            const string filter = "";
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.ListarDatos(datosUsuario, filter) as ViewResult;
            IEnumerable<InhabilitacionCamionDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearInhabilitacionCamion>()))
                .Returns(new Resultado());

            var inhabilitacionCamionDto = new InhabilitacionCamionDto
            {
                Id = 1,
                Patente = "AAA123",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                Motivo = "m",
                Adjuntos = new List <AdjuntoDto>()
            };
            
            const string json = "[]";
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, json, inhabilitacionCamionDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            const string json = "[]";
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearInhabilitacionCamion>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarAdjuntosCamion(1))
                .Returns(new List<AdjuntoDto> { new AdjuntoDto { Archivo = "archivo" }, new AdjuntoDto(), new AdjuntoDto() });

            var inhabilitacionCamionDto = new InhabilitacionCamionDto
            {
                Id = 1,
                Patente = "AAA123",
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                Motivo = "m",
                Adjuntos = new List<AdjuntoDto>()
            };
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, json, inhabilitacionCamionDto) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarInhabilitacionCamion>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarInhabilitacionCamion>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
