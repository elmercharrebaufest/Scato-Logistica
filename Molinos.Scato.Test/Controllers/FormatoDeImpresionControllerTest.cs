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
    public class FormatoDeImpresionControllerTest
    {
        private FormatoDeImpresionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<FormatoDeImpresionDto> impresiones;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new FormatoDeImpresionController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            impresiones = new List<FormatoDeImpresionDto>
                {
                    new FormatoDeImpresionDto
                        {
                            Id = 1,
                            Descripcion = "Descripcion",
                        },
                    new FormatoDeImpresionDto
                        {
                            Id = 2,
                            Descripcion = "Descripcion",
                        }
                };
            servRepositorioMock.Setup(s => s.ListarLetras())
               .Returns(new List<LetraDto> { new LetraDto() { Id = 1, Descripcion = "a" }, new LetraDto() { Id = 2, Descripcion = "b" } });
            servRepositorioMock.Setup(s => s.ListarCampos())
               .Returns(new List<CampoDto> { new CampoDto() { Id = 1, Descripcion = "a" }, new CampoDto() { Id = 2, Descripcion = "b" } });
            servRepositorioMock.Setup(s => s.ListarFormatosDePapel())
               .Returns(new List<FormatoDePapelDto> { new FormatoDePapelDto() { Id = 1, Descripcion = "a" }, new FormatoDePapelDto() { Id = 2, Descripcion = "b" } }); 

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearFormatoDeImpresion>()))
                .Returns(new Resultado());

            var dto = new FormatoDeImpresionDto
                {
                    Descripcion = "Descripcion",
                };

            var result = target.Crear("", dto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerFormatoDeImpresion(It.IsAny<int>()))
                .Returns(new FormatoDeImpresionDto()
                            {
                                Id = 1,
                                Descripcion = "Descripcion"
                            } );

            var result = target.Modificar(1) as ViewResult;


            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarFormatoDeImpresion>()))
                .Returns(new Resultado());

            var dto = new FormatoDeImpresionDto
            {
                Descripcion = "Descripcion",
            };

            var result = target.Modificar("", dto, datosUsuario) as ContentResult;
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarFormatoDeImpresion>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }
    }
}
