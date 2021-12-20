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
    public class RangosDeRedondeoControllerTest
    {
        private RangosDeRedondeoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<RangosDeRedondeoDto> rangos;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new RangosDeRedondeoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            rangos = new List<RangosDeRedondeoDto>
                {
                    new RangosDeRedondeoDto
                        {
                            Id = 1,
                            MaterialPorCentroDescripcion = "a",
                            MaterialPorCentroId = 5,
                            ValorDesde = 5,
                            ValorHasta = 7,
                            ValorRedondeado = 6
                        },
                    new RangosDeRedondeoDto
                        {
                            Id = 2,
                            MaterialPorCentroDescripcion = "b",
                            MaterialPorCentroId = 26,
                            ValorDesde = 25,
                            ValorHasta = 27,
                            ValorRedondeado = 26
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRangosDeRedondeo(It.IsAny<string>(),It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RangosDeRedondeoDto>(rangos, 1,2,2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<RangosDeRedondeoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].MaterialPorCentroDescripcion, Is.EqualTo("a"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoRangosDeRedondeo(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<RangosDeRedondeoDto>(rangos, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<RangosDeRedondeoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].MaterialPorCentroDescripcion, Is.EqualTo("a"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarMaterialesPorCentro(datosUsuario.CentroId))
                .Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto { MaterialDesc = "b" }, new MaterialPorCentroDto { MaterialDesc = "c"}, new MaterialPorCentroDto { MaterialDesc = "d"} });

            var result = target.Crear(datosUsuario) as ViewResult;
            List<SelectListItem> materiales = target.ViewBag.Materiales;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(materiales.Count(), Is.EqualTo(3));
            Assert.That(materiales[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRangosDeRedondeo>()))
                .Returns(new Resultado());
            
            var rangosDeRedondeoDto = new RangosDeRedondeoDto
            {
                Id = 1,
                MaterialPorCentroDescripcion = "c",
                MaterialPorCentroId = 36,
                ValorDesde = 35,
                ValorHasta = 37,
                ValorRedondeado = 36
            };

            var result = target.Crear(rangosDeRedondeoDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarMaterialesPorCentro(datosUsuario.CentroId))
                .Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto { MaterialDesc = "b" }, new MaterialPorCentroDto { MaterialDesc = "c" }, new MaterialPorCentroDto { MaterialDesc = "d" } });

            var resultado = new Resultado();
            resultado.Error("Error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRangosDeRedondeo>())).Returns(resultado);

            var rangosDeRedondeoDto = new RangosDeRedondeoDto
            {
                Id = 1,
                MaterialPorCentroDescripcion = "c",
                MaterialPorCentroId = 36,
                ValorDesde = 35,
                ValorHasta = 37,
                ValorRedondeado = 36
            };

            var result = target.Crear(rangosDeRedondeoDto, datosUsuario) as ViewResult;

            List<SelectListItem> materiales = target.ViewBag.Materiales;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(materiales.Count(), Is.EqualTo(3));
            Assert.That(materiales[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarMaterialesPorCentro(datosUsuario.CentroId))
                 .Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto { MaterialDesc = "b" }, new MaterialPorCentroDto { MaterialDesc = "c" }, new MaterialPorCentroDto { MaterialDesc = "d" } });
            servRepositorioMock.Setup(s => s.ObtenerRangosDeRedondeo(It.IsAny<int>()))
                .Returns(new RangosDeRedondeoDto
                {
                    Id = 1,
                    MaterialPorCentroDescripcion = "c",
                    MaterialPorCentroId = 36,
                    ValorDesde = 35,
                    ValorHasta = 37,
                    ValorRedondeado = 36
                } );

            var result = target.Modificar(1, datosUsuario) as ViewResult;
            List<SelectListItem> materiales = target.ViewBag.Materiales;

            Assert.NotNull(result.Model);
            Assert.That(materiales.Count(), Is.EqualTo(3));
            Assert.That(materiales[0].Text, Is.EqualTo("b"));
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRangosDeRedondeo>()))
                .Returns(new Resultado());

            var rangosDeRedondeoDto = new RangosDeRedondeoDto
            {
                Id = 1,
                MaterialPorCentroDescripcion = "c",
                MaterialPorCentroId = 36,
                ValorDesde = 35,
                ValorHasta = 37,
                ValorRedondeado = 36
            };

            var result = target.Modificar(rangosDeRedondeoDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarMaterialesPorCentro(datosUsuario.CentroId))
                 .Returns(new List<MaterialPorCentroDto> { new MaterialPorCentroDto { MaterialDesc = "b" }, new MaterialPorCentroDto { MaterialDesc = "c" }, new MaterialPorCentroDto { MaterialDesc = "d" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRangosDeRedondeo>())).Returns(resultado);

            var rangosDeRedondeoDto = new RangosDeRedondeoDto
            {
                Id = 1,
                MaterialPorCentroDescripcion = "c",
                MaterialPorCentroId = 36,
                ValorDesde = 35,
                ValorHasta = 37,
                ValorRedondeado = 36
            };

            var result = target.Modificar(rangosDeRedondeoDto, datosUsuario) as ViewResult;

            List<SelectListItem> materiales = target.ViewBag.Materiales;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(materiales.Count(), Is.EqualTo(3));
            Assert.That(materiales[0].Text, Is.EqualTo("b"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarRangosDeRedondeo>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarRangosDeRedondeo>())).Returns(resultado);

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
