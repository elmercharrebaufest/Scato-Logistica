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
    public class AjusteDeStockControllerTest
    {
        private AjusteDeStockController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<AjusteDeStockDto> ajustes;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new AjusteDeStockController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            ajustes = new List<AjusteDeStockDto>
                {
                    new AjusteDeStockDto
                        {
                            Id = 1,
                            CentroId = 1,
                            Fecha = DateTime.Now,
                            MaterialId = 1,
                            PesoBrutoIngreso = 0,
                            PesoNetoEgreso = 0,
                            PesoNetoIngreso = 1,
                            TipoComprobanteOnccaId = 1,
                            NumeroDocumentoIngreso = "1234-12345678"
                        },
                    new AjusteDeStockDto
                        {
                            Id = 2,
                            CentroId = 1,
                            Fecha = DateTime.Now,
                            MaterialId = 1,
                            PesoBrutoIngreso = 0,
                            PesoNetoEgreso = 0,
                            PesoNetoIngreso = 1,
                            TipoComprobanteOnccaId = 1,
                            NumeroDocumentoIngreso = "1234-12345678"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoAjusteDeStock(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<AjusteDeStockDto>(ajustes, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter, new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<AjusteDeStockDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NumeroDocumentoIngreso, Is.EqualTo("1234-12345678"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoAjusteDeStock(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<AjusteDeStockDto>(ajustes, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter, new DatosUsuario{CentroId = 1}) as ViewResult;
            IEnumerable<AjusteDeStockDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NumeroDocumentoIngreso, Is.EqualTo("1234-12345678"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearAjusteDeStock>())).Returns(new Resultado());

            var ajusteDto = new AjusteDeStockDto()
            {
                Id = 1,
                CentroId = 1,
                Fecha = DateTime.Now,
                MaterialId = 1,
                PesoBrutoIngreso = 0,
                PesoNetoEgreso = 0,
                PesoNetoIngreso = 1,
                TipoComprobanteOnccaId = 1,
                NumeroDocumentoIngreso = "1234-12345678"
            };
            var datos = new DatosUsuario()
                {

                    CentroId = 1
                };

            var result = target.Crear(ajusteDto, datos) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComprobantesOncca())
                .Returns(new List<TipoComprobanteOnccaDto> { new TipoComprobanteOnccaDto(), new TipoComprobanteOnccaDto(), new TipoComprobanteOnccaDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearAjusteDeStock>())).Returns(resultado);

            var ajusteDto = new AjusteDeStockDto()
            {
                Id = 1,
                CentroId = 1,
                Fecha = DateTime.Now,
                MaterialId = 1,
                PesoBrutoIngreso = 0,
                PesoNetoEgreso = 0,
                PesoNetoIngreso = 1,
                TipoComprobanteOnccaId = 1,
                NumeroDocumentoIngreso = "1234-12345678"
            };
            var datos = new DatosUsuario()
            {

                CentroId = 1
            };

            var result = target.Crear(ajusteDto, datos) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarAjusteDeStock>()))
                .Returns(new Resultado());

            var ajusteDto = new AjusteDeStockDto()
            {
                Id = 1,
                CentroId = 1,
                Fecha = DateTime.Now,
                MaterialId = 1,
                PesoBrutoIngreso = 0,
                PesoNetoEgreso = 0,
                PesoNetoIngreso = 1,
                TipoComprobanteOnccaId = 1,
                NumeroDocumentoIngreso = "1234-12345678"
            };

            var result = target.Modificar(ajusteDto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComprobantesOncca())
                .Returns(new List<TipoComprobanteOnccaDto> { new TipoComprobanteOnccaDto(), new TipoComprobanteOnccaDto(), new TipoComprobanteOnccaDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarAjusteDeStock>())).Returns(resultado);

            var ajusteDto = new AjusteDeStockDto()
            {
                Id = 1,
                CentroId = 1,
                Fecha = DateTime.Now,
                MaterialId = 1,
                PesoBrutoIngreso = 0,
                PesoNetoEgreso = 0,
                PesoNetoIngreso = 1,
                TipoComprobanteOnccaId = 1,
                NumeroDocumentoIngreso = "1234-12345678"
            };

            var result = target.Modificar(ajusteDto, new DatosUsuario()) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarAjusteDeStock>())).Returns(new Resultado());

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarAjusteDeStock>())).Returns(resultado);

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
