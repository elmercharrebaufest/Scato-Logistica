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
    public class TarjetaBloqueadaControllerTest
    {
        private TarjetaBloqueadaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TarjetaBloqueadaDto> tarjetasBloqueadas;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TarjetaBloqueadaController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            tarjetasBloqueadas = new List<TarjetaBloqueadaDto>
                {
                    new TarjetaBloqueadaDto
                        {
                            Id = 1,
                            Numero = "11111"
       
                        },
                    new TarjetaBloqueadaDto
                        {
                            Id = 2,
                            Numero = "22222"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTarjetasBloqueadas(It.IsAny<string>(), It.IsAny<Paginacion>(), It.Is<int>(i => i != 0))).Returns(new ListaPaginada<TarjetaBloqueadaDto>(new List<TarjetaBloqueadaDto>(), 1, 2, 2));

            var result = target.Index(new DatosUsuario{CentroId = 1},It.IsAny<string>()) as ViewResult;
            IEnumerable<TarjetaBloqueadaDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>()));
        }


        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTarjetasBloqueadas(It.IsAny<string>(), It.IsAny<Paginacion>(), It.Is<int>(i => i != 0))).Returns(new ListaPaginada<TarjetaBloqueadaDto>(tarjetasBloqueadas, 1, 2, 2));
            var result = target.Index(new DatosUsuario{CentroId = 1}, "") as ViewResult;
            IEnumerable<TarjetaBloqueadaDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>{1,2}));
        }

        [Test]
        public void TestBloquear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Bloquear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(((TarjetaBloqueadaDto)result.Model).CentroId, Is.EqualTo(datosUsuario.CentroId));
        }

        [Test]
        public void TestBloquearPost()
        {
            var modelo = new TarjetaBloqueadaDto
                {
                    Numero = "33333",
                    Motivo = "Motivo 1"
                };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<BloquearTarjeta>())).Returns(new Resultado());
            var result = target.Bloquear(modelo) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<BloquearTarjeta>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestBloquearPostInvalido()
        {
            var modelo = new TarjetaBloqueadaDto
            {
                Numero = "33333",
                Motivo = "Motivo 1"
            };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<BloquearTarjeta>())).Returns(resultado);
            var result = target.Bloquear(modelo) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTarjetaBloqueada>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<EliminarTarjetaBloqueada>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
