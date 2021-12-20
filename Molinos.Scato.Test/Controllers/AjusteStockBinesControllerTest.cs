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
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AjusteStockBinesControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private NullLogger log;
        private AjusteStockBinesController target;
        private AjusteStockBinesDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            log = new NullLogger();
            target = new AjusteStockBinesController(log,servComando.Object, servRepositorio.Object);
            dto = new AjusteStockBinesDto
                {
                    Id = 1,
                    CentroId = 1,
                    CentroDescripcion = "C",
                    Stock = 2,
                    Fecha = new DateTime(2015, 5, 5),
                    MaterialId = 1,
                    Observaciones = "Obs"
                };

            servRepositorio.Setup(s => s.ListarMaterialesBinPallet())
                           .Returns(new List<MaterialDto>
                               {
                                   new MaterialDto {Id = 1, Activo = true, UsaBinPallet = true, Descripcion = "M"}
                               });
            servRepositorio.Setup(
                s => s.ListarPaginadoAjusteYStockBines(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                           .Returns(
                               new ListaPaginada<AjusteStockBinesDto>(
                                   new List<AjusteStockBinesDto>
                                       {
                                          dto
                                       }, 1, 1, 1));

            servRepositorio.Setup(s => s.ObtenerAjusteYStockBines(It.IsAny<int>())).Returns(dto);
        }

        [Test]
        public void IndexTest()
        {
            var resultado = target.Index("", new DatosUsuario {CentroId = 1, NombreUsuario = "wandino"}) as ViewResult;
            
            Assert.NotNull(resultado);
            IEnumerable<AjusteStockBinesDto> resultados = target.ViewBag.Items;

            Assert.That(resultado.ViewName, Is.Null.Or.Empty);
            Assert.That(resultados.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1}));
           
        }

        [Test]
        public void CrearTest()
        {
            var resultado = target.Crear() as ViewResult;

            Assert.IsNotNull(resultado);
            Assert.AreEqual(((List<SelectListItem>) (resultado.ViewData.Values.ElementAt(0))).ElementAt(0).Text, "M");
        }

        [Test]
        public void ListarTest()
        {
            var result = target.Listar("", new DatosUsuario { CentroId = 1 }) as ViewResult;
            IEnumerable<AjusteStockBinesDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1 }));
            
        }

        [Test]
        public void CrearPostTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearAjusteStockBines>())).Returns(new Resultado());

            var datos = new DatosUsuario()
            {
                CentroId = 1
            };

            var result = target.Crear(dto, datos) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void CrearPostInvalidoTest()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComando.Setup(s => s.Ejecutar(It.IsAny<CrearAjusteStockBines>())).Returns(resultado);

            var datos = new DatosUsuario()
            {

                CentroId = 1
            };

            var result = target.Crear(dto, datos) as ViewResult;

            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void ModificarPostTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<ModificarAjusteStockBines>()))
                .Returns(new Resultado());

           
            var result = target.Modificar(dto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComando.Setup(s => s.Ejecutar(It.IsAny<ModificarAjusteStockBines>())).Returns(resultado);

            var result = target.Modificar(dto, new DatosUsuario()) as ViewResult;

            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
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

            servComando.Setup(s => s.Ejecutar(It.IsAny<EliminarAjusteStockBines>())).Returns(new Resultado());

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
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
            servComando.Setup(s => s.Ejecutar(It.IsAny<EliminarAjusteStockBines>())).Returns(resultado);

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

            servComando.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
