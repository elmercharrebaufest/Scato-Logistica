using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class FirmaControllerTest
    {
        private FirmaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IFirmaProvider> firmaProviderMock;
        private Mock<HttpPostedFileBase> archivosMock;

        private Mock<IServicioComandosFactory> factoryMock;
        private Mock<IConfiguracionProvider> configuracionMock;

        private FirmaDto firma;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            archivosMock = new Mock<HttpPostedFileBase>();
            firmaProviderMock = new Mock<IFirmaProvider>();

            factoryMock = new Mock<IServicioComandosFactory>();
            configuracionMock = new Mock<IConfiguracionProvider>();

            target = new FirmaController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object, firmaProviderMock.Object, configuracionMock.Object, factoryMock.Object);

            configuracionMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "servurl" } });
            factoryMock.Setup(x => x.CrearServicio(It.IsAny<string>())).Returns(servComandosMock.Object);


            archivosMock.Setup(d => d.FileName).Returns("test1.txt");
            archivosMock.Setup(d => d.InputStream).Returns(new MemoryStream(new byte[1000]));
            firma = new FirmaDto
            {
                Descripcion = "Molinos",
                CodigoSAP = "9999",
                Cuit = "11-11111111-3",
                Id = 1,
                Logo = new byte[0],
                RazonSocial = "MOLINOS RIO PLATA",
                LogoFile = archivosMock.Object
            };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerFirma()).Returns(firma);
            var result = target.Index() as ViewResult;
            servRepositorioMock.Verify(s => s.ObtenerFirma(),Times.Exactly(1));

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificarPost()
        {           
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarFirma>())).Returns(new Resultado());

            var result = target.Index(new DatosUsuario { NombreUsuario = "Jose" }, firma) as ViewResult;
            var resultadoOperacion = target.ViewBag.ResultadoOperacion;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(resultadoOperacion, Is.EqualTo("Exitosa"));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestModificarPostError()
        {
            var res = new Resultado();
            res.Error("logo","invalido");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarFirma>())).Returns(res);
            var result = target.Index(new DatosUsuario { NombreUsuario = "Jose" }, firma) as ViewResult;
            var resultadoOperacion = target.ViewBag.ResultadoOperacion;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(resultadoOperacion, Is.Null);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            
        }
    }
}
