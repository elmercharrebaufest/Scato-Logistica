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
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ConversionGrupoControllerTest
    {
        private ConversionGrupoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ConversionGrupoDto> conversionGrupo;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConversionGrupoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            conversionGrupo = new List<ConversionGrupoDto>
                {
                    new ConversionGrupoDto
                        {
                            Id = 1,
                            CamaraId = 1,
                        },
                    new ConversionGrupoDto
                        {
                            Id = 2,
                            CamaraId = 2,
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoConversionGrupo(It.IsAny<Paginacion>(), null))
                .Returns(new ListaPaginada<ConversionGrupoDto>(conversionGrupo, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarCamaras())
            .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "b" }, new CamaraDto { Id = 2, Descripcion = "c" }, new CamaraDto { Id = 3, Descripcion = "d" } });

            servRepositorioMock.Setup(s => s.ListarMaterialesPorCamara(0))
            .Returns(new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "a" }, new MaterialDto { Id = 2, Descripcion = "b" } });

            var result = target.Index(null, 1,"Id", DirOrden.Asc) as ViewResult;
            IEnumerable<ConversionGrupoDto> results = target.ViewBag.Items;
            List<SelectListItem> camaras = target.ViewBag.Camaras;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoConversionGrupo(It.IsAny<Paginacion>(), null))
                .Returns(new ListaPaginada<ConversionGrupoDto>(conversionGrupo, 1, 2, 2));

            servRepositorioMock.Setup(s => s.ListarCamaras())
           .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "b" }, new CamaraDto { Id = 2, Descripcion = "c" }, new CamaraDto { Id = 3, Descripcion = "d" } });

            servRepositorioMock.Setup(s => s.ListarMaterialesPorCamara(0))
            .Returns(new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "a" }, new MaterialDto { Id = 2, Descripcion = "b" } });

            var result = target.Index(null, 1,"Id", DirOrden.Asc) as ViewResult;
            IEnumerable<ConversionGrupoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearConversionGrupo>()))
                .Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "b" }, new CamaraDto { Id = 2, Descripcion = "c" }, new CamaraDto { Id = 3, Descripcion = "d" } });
            

            var conversionGrupoDto = new ConversionGrupoDto
            {
                Id = 1,
                CamaraId = 1,
            };

            var result = target.Crear(conversionGrupoDto) as ContentResult;
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionGrupo>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarConversionGrupo>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
