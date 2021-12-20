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
    public class DocumentoDeImpresionControllerTest
    {
        private DocumentoDeImpresionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<DocumentoDeImpresionDto> impresiones;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new DocumentoDeImpresionController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            impresiones = new List<DocumentoDeImpresionDto>
                {
                    new DocumentoDeImpresionDto
                        {
                            Id = 1,
                            Codigo = "Codigo",
                            Descripcion = "Descripcion",
                            DescripcionCorta = "DescripcionCorta"
                        },
                    new DocumentoDeImpresionDto
                        {
                            Id = 2,
                            Codigo = "Codigo2",
                            Descripcion = "Descripcion",
                            DescripcionCorta = "DescripcionCorta"
                        }
                };
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearDocumentoDeImpresion>()))
                .Returns(new Resultado());

            var dto = new DocumentoDeImpresionDto
                {
                    Codigo = "Codigo2",
                    Descripcion = "Descripcion",
                    DescripcionCorta = "DescripcionCorta"
                };

            var result = target.Crear(dto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerImpresiones(It.IsAny<int>()))
                .Returns(new DocumentoDeImpresionDto()
                            {
                                Id = 1,
                                Codigo = "Codigo2",
                                Descripcion = "Descripcion",
                                DescripcionCorta = "DescripcionCorta"
                            } );

            var result = target.Modificar(1) as ViewResult;


            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarDocumentoDeImpresion>()))
                .Returns(new Resultado());

            var dto = new DocumentoDeImpresionDto
            {
                Codigo = "Codigo2",
                Descripcion = "Descripcion",
                DescripcionCorta = "DescripcionCorta"
            };

            var result = target.Modificar(dto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarDocumentoDeImpresion>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }
    }
}
