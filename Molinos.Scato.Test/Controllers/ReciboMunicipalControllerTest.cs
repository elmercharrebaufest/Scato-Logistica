using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ReciboMunicipalControllerTest
    {
        private ReciboMunicipalController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private ReciboMunicipalDto reciboMunicipal;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ReciboMunicipalController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            reciboMunicipal = new ReciboMunicipalDto { Id = 1, Monto = 50, Ordenanza = "Ordenanza", CentroId = 1};
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerReciboMunicipal(It.IsAny<int>())).Returns(reciboMunicipal);
            
            var datosUsuario = new DatosUsuario{CentroId = 1};
            var result = target.Index(datosUsuario, It.IsAny<string>()) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestIndexPostCrearRecibo()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearReciboMunicipal>()))
               .Returns(new Resultado());

            var reciboDto = new ReciboMunicipalDto
            {
                Id = 1,
                CentroId = 5,
                Monto = 400,
                Ordenanza = "uno"
            };

            var result = target.Crear(reciboDto, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestIndexPostModificarRecibo()
        {
            servRepositorioMock.Setup(s => s.ObtenerReciboMunicipal(It.IsAny<int>()))
                .Returns(new ReciboMunicipalDto
                {
                    Id = 1,
                    CentroId = 5,
                    Monto = 400,
                    Ordenanza = "uno"
                });

            var result = target.Modificar(1) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestIndexPostInvalido()
        {

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarProveedor>()))
                 .Returns(new Resultado());
            var reciboDto = new ReciboMunicipalDto
            {
                Id = 1,
                Ordenanza = "Recibo modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(1) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.IsNull(result.Model);
            Assert.IsNull(result.View);
        }
    }
}
