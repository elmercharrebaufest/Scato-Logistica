using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ConversionProcedenciaControllerTest
    {
        private ConversionProcedenciaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ConversionProcedenciaDto> conversionProcedencia;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConversionProcedenciaController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            conversionProcedencia = new List<ConversionProcedenciaDto>
                {
                    new ConversionProcedenciaDto
                        {
                            Id = 1,
                            ProcedenciaId = 1,
                            CamaraId = 1,
                            CodigoCamara = "1"
                        },
                    new ConversionProcedenciaDto
                        {
                            Id = 2,
                            ProcedenciaId = 2,
                            CamaraId = 2,
                            CodigoCamara = "2"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoConversionProcedencia(It.IsAny<Paginacion>(), null))
                .Returns(new ListaPaginada<ConversionProcedenciaDto>(conversionProcedencia, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarCamaras())
            .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "b" }, new CamaraDto { Id = 2, Descripcion = "c" }, new CamaraDto { Id = 3, Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarLocalidades())
            .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1, Descripcion = "a" }, new LocalidadDto { Id = 2, Descripcion = "b"} });

            var result = target.Index(1, 1, "Id", DirOrden.Asc) as ViewResult;
            IEnumerable<ConversionProcedenciaDto> results = target.ViewBag.Items;
            List<SelectListItem> Procedencias = target.ViewBag.Procedencias;
            List<SelectListItem> camaras = target.ViewBag.Camaras;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearConversionProcedencia>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarCamaras())
            .Returns(new List<CamaraDto>{});
            servRepositorioMock.Setup(s => s.ListarLocalidades())
            .Returns(new List<LocalidadDto> {});

            var conversionProcedenciaDto = new ConversionProcedenciaDto
            {
                Id = 1,
                ProcedenciaId = 1,
                CamaraId = 1,
                CodigoCamara = "1"
            };

            var result = target.Crear(conversionProcedenciaDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionProcedencia>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionProcedencia>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
