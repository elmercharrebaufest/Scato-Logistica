using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class SugerenciaControllerTest
    {
        private SugerenciaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private NullLogger logger;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            logger = new NullLogger();
            target = new SugerenciaController(logger, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario{CentroId = 1};
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index() as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void IndexPostTest()
        {
            var datosUsuario = new DatosUsuario() {CentroId = 1, NombreUsuario = "User 1"};
            var textoSugerencia = "Sugerencia 1";

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://www.site.com"));
            context.Setup(c => c.Request).Returns(request.Object);
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<CrearSugerencia>())).Returns(new Resultado());

            var result = target.Index(datosUsuario, textoSugerencia) as ContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.Content, expectedResult.Content);
            servComandosMock.Verify(x => x.Ejecutar(It.IsAny<CrearSugerencia>()), Times.Exactly(1));
        }

        [Test]
        public void IndexPostInvalidoTest()
        {
            var datosUsuario = new DatosUsuario() { CentroId = 1, NombreUsuario = "User 1" };
            var textoSugerencia = "Sugerencia 1";

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://www.site.com"));
            context.Setup(c => c.Request).Returns(request.Object);

            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);


            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<CrearSugerencia>())).Returns(new Resultado());
            target.ModelState.AddModelError("", "Error");
            var result = target.Index(datosUsuario, textoSugerencia) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.View, Is.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
        }
    }
}
