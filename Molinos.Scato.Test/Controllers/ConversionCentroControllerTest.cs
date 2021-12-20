using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ConversionCentroControllerTest
    {
        private ConversionCentroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ConversionCentroDto> conversioncentro;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConversionCentroController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            conversioncentro = new List<ConversionCentroDto>
                {
                    new ConversionCentroDto
                        {
                            Id = 1,
                            CentroId = 1,
                            CamaraId = 1,
                            CodigoCamara = "1"
                        },
                    new ConversionCentroDto
                        {
                            Id = 2,
                            CentroId = 2,
                            CamaraId = 2,
                            CodigoCamara = "2"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoConversionCentro(It.IsAny<int?>(), null))
                               .Returns(new ListaPaginada<ConversionCentroDto>(conversioncentro, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarCamaras())
                               .Returns(new List<CamaraDto>
                                   {
                                       new CamaraDto {Id = 1, Descripcion = "b"},
                                       new CamaraDto {Id = 2, Descripcion = "c"},
                                       new CamaraDto {Id = 3, Descripcion = "d"}
                                   });
            servRepositorioMock.Setup(s => s.ListarCentros())
                               .Returns(new List<CentroDto>
                                   {
                                       new CentroDto() {Id = 1, Descripcion = "a"},
                                       new CentroDto() {Id = 2, Descripcion = "b"}
                                   });

            var result = target.Index(1) as ViewResult;
            IEnumerable<ConversionCentroDto> results = target.ViewBag.Items;
            
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearConversionCentro>()))
                            .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarCamaras())
                               .Returns(new List<CamaraDto> {});
            servRepositorioMock.Setup(s => s.ListarCentros())
                               .Returns(new List<CentroDto> {});

            var conversionCentroDto = new ConversionCentroDto
                {
                    Id = 1,
                    CentroId = 1,
                    CamaraId = 1,
                    CodigoCamara = "1"
                };

            var result = target.Crear(conversionCentroDto) as ContentResult;
            var expectedResult = new ContentResult {Content = "ajax-edit-success"};

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionCentro>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionCentro>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
