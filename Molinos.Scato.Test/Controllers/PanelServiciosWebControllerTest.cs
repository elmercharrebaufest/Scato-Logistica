using System.Collections.Specialized;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class PanelServiciosWebControllerTest
    {
        private PanelServiciosWebController target;
        private Mock<IConfiguracionProvider> configuracion;
        private Mock<IServicioRepositorio> serviciomMock;
        private Mock<IListaDeWorkflows> listaMock;
        private Mock<ZSDWS_SCATO> sapMock;
        private Mock<IServicioNotificarUsuario> notificarMock;
        private Mock<IServicioComandos> comandosMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            configuracion = new Mock<IConfiguracionProvider>();
            serviciomMock = new Mock<IServicioRepositorio>();
            listaMock = new Mock<IListaDeWorkflows>();
            sapMock = new Mock<ZSDWS_SCATO>();
            notificarMock = new Mock<IServicioNotificarUsuario>();
            comandosMock = new Mock<IServicioComandos>();
            log = new NullLogger();
            target = new PanelServiciosWebController(configuracion.Object, log, serviciomMock.Object, listaMock.Object, notificarMock.Object, sapMock.Object, comandosMock.Object);


        }

        [Test]
        public void Index()
        {

            configuracion.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "http://bfdev258/Scato.serviciosWeb/|http://bfdev260/Scato.serviciosWeb/" } });

            var resultado = target.Index() as ViewResult;


            Assert.NotNull(resultado);
        }
    }
}
