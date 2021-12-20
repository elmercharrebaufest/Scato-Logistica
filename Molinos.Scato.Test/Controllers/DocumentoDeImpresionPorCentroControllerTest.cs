using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
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
    public class DocumentoDeImpresionPorCentroControllerTest
    {
        private DocumentoDeImpresionPorCentroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<DocumentoDeImpresionPorCentroDto> DocumentoDeImpresionPorCentro;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new DocumentoDeImpresionPorCentroController(
                null, servRepositorioMock.Object, servComandosMock.Object);
            target.ModelState.Add("PuestoDeTrabajoId",new ModelState() );
            DocumentoDeImpresionPorCentro = new List<DocumentoDeImpresionPorCentroDto>
                {
                    new DocumentoDeImpresionPorCentroDto
                        {
                            Id = 1
                        },
                    new DocumentoDeImpresionPorCentroDto
                        {
                            Id = 2
                        }
                };
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarFormatosDeImpresion())
                .Returns(new List<FormatoDeImpresionDto> { new FormatoDeImpresionDto() { Id = 1, Descripcion = "a" }, new FormatoDeImpresionDto() { Id = 2, Descripcion = "b" } });
            servRepositorioMock.Setup(s => s.ListarDocumentosDeImpresion())
                .Returns(new List<DocumentoDeImpresionDto> { new DocumentoDeImpresionDto() { Id = 1, Descripcion = "b" }, new DocumentoDeImpresionDto { Id = 2, Descripcion = "c" }, new DocumentoDeImpresionDto { Id = 3, Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarImpresoras(It.IsAny<int>()))
                .Returns(new List<ImpresoraDto> { new ImpresoraDto { Id = 1, Descripcion = "b" }, new ImpresoraDto { Id = 2, Descripcion = "c" }, new ImpresoraDto { Id = 3, Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>())).Returns(new List<PuestoDeTrabajoDto>{new PuestoDeTrabajoDto {Id = 1, NombrePuesto = "Puesto 1"}});
            var result = target.Crear(new DatosUsuario()) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearDocumentoDeImpresionPorCentro>()))
                .Returns(new Resultado());

            var dto = new DocumentoDeImpresionPorCentroDto
                {
                    CentroId = 1
                };

            var result = target.Crear(dto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarFormatosDeImpresion())
               .Returns(new List<FormatoDeImpresionDto> { new FormatoDeImpresionDto() { Id = 1, Descripcion = "a" }, new FormatoDeImpresionDto() { Id = 2, Descripcion = "b" } });
            servRepositorioMock.Setup(s => s.ListarDocumentosDeImpresion())
                .Returns(new List<DocumentoDeImpresionDto> { new DocumentoDeImpresionDto() { Id = 1, Descripcion = "b" }, new DocumentoDeImpresionDto { Id = 2, Descripcion = "c" }, new DocumentoDeImpresionDto { Id = 3, Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarImpresoras(It.IsAny<int>()))
                .Returns(new List<ImpresoraDto> { new ImpresoraDto { Id = 1, Descripcion = "b" }, new ImpresoraDto { Id = 2, Descripcion = "c" }, new ImpresoraDto { Id = 3, Descripcion = "d" } });

            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(It.IsAny<int>())).Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "Puesto 1" } });
            servRepositorioMock.Setup(s => s.ObtenerDocumentoDeImpresionPorCentro(It.IsAny<int>()))
                .Returns(new DocumentoDeImpresionPorCentroDto()
                            {
                                Id = 1,
                            } );

            var result = target.Modificar(1,new DatosUsuario()) as ViewResult;


            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarDocumentoDeImpresionPorCentro>()))
                .Returns(new Resultado());

            var dto = new DocumentoDeImpresionPorCentroDto
            {
                CentroId = 1
            };

            var result = target.Modificar(dto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestEliminar()
        {
            var datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarDocumentoDeImpresionPorCentro>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }
    }
}
