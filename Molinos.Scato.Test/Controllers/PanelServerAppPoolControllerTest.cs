using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class PanelServerAppPoolControllerTest
    {
        private PanelServerAppPoolController target;
        private Mock<IConfiguracionProvider> configuracion;
        private Mock<IServicioRepositorioFactory> servicioFactory;
        private Mock<IServicioRepositorio> serviciomMock;
        private ILogger log;

        [SetUp]
        public void SetUp()
        {
            servicioFactory = new Mock<IServicioRepositorioFactory>();
            configuracion = new Mock<IConfiguracionProvider>();
            serviciomMock = new Mock<IServicioRepositorio>();
            log = new NullLogger();

            target = new PanelServerAppPoolController(configuracion.Object, servicioFactory.Object, log, serviciomMock.Object);

            configuracion.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "HostsServiciosWeb", "http://bfdev258/Scato.serviciosWeb/|http://bfdev260/Scato.serviciosWeb/" } });
            servicioFactory.Setup(s => s.CrearServicio(It.IsAny<string>())).Returns(serviciomMock.Object);
        }

        [Test]
        public void Index()
        {
            
            serviciomMock.Setup(s => s.ObtenerEstadoServidor(It.IsAny<string>())).Returns(new PanelServerAppPoolDto());
            

            var resultado = target.Index() as ViewResult;


            Assert.NotNull(resultado);
        }

        [Test]
        public void DetenerAppPool()
        {

            serviciomMock.Setup(s => s.DetenerAppPool(It.IsAny<string>()));


            var resultado = target.DetenerAppPool("bfdev258", It.IsAny<string>());


            Assert.NotNull(resultado);
        }

        [Test]
        public void IniciarAppPool()
        {

            serviciomMock.Setup(s => s.IniciarAppPool(It.IsAny<string>()));


            var resultado = target.IniciarAppPool("bfdev258", It.IsAny<string>());


            Assert.NotNull(resultado);
        }
        
        [Test]
        public void ReciclarAppPool()
        {

            serviciomMock.Setup(s => s.ReciclarAppPool(It.IsAny<string>()));


            var resultado = target.ReciclarAppPool("bfdev258", It.IsAny<string>());


            Assert.NotNull(resultado);
        }

        [Test]
        public void ConfigurarReciclado()
        {

            serviciomMock.Setup(s => s.ConfigurarTiempoReciclado(It.IsAny<string>(), It.IsAny<double>()));


            var resultado = target.ConfigurarReciclado("bfdev258", It.IsAny<string>(), It.IsAny<double>());

            Assert.NotNull(resultado);

        }

        [Test]
        public void ObtenerEventos()
        {

            serviciomMock.Setup(s => s.ObtenerLogEventos(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new List<EventLogEntry>());
            serviciomMock.Setup(s => s.ObtenerEstadoServidor(It.IsAny<string>())).Returns(new PanelServerAppPoolDto());

            var resultado = target.ObtenerEventos("bfdev258", It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()) as PartialViewResult;

            Assert.NotNull(resultado);

        }

    }
}
