using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.WebMobile.Controllers;
using Moq;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Molinos.Scato.Test.ControllersMobile
{
    [TestFixture]
    public class TiempoEnTransitoControllerTest
    {
        private CupoController target;
        private Mock<IServicioRepositorio> servicio;
        private Mock<IServicioComandos> servicioComandos;
        private Mock<IConfiguracionProvider> configuracion;
        private  NullLogger log;

        [SetUp]
        public void SetUp()
        {
            servicio = new Mock<IServicioRepositorio>();
            servicioComandos = new Mock<IServicioComandos>();
            configuracion = new Mock<IConfiguracionProvider>();
            log = new NullLogger();
            target = new CupoController(log, servicio.Object, servicioComandos.Object, configuracion.Object);
            configuracion.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "TiemposCuposOtorgados", "5" } });

        }



    }
}
