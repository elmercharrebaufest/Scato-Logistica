using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
    public class ReporteControlDeBalanzaControllerTest
    {
        private ReporteControlDeBalanzaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            logger = new NullLogger();
            target = new ReporteControlDeBalanzaController(logger, servRepositorioMock.Object);
        }
        [Test]
        public void TestIndex()
        {
            var result = target.Index() as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarControlDeBalanza(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new List<ControlDeBalanzaPesadaDto>());
            var result = target.Listar(new FechaModel { FechaDesde = new DateTime(2014, 1, 1), FechaHasta = new DateTime(2014, 1, 1) }) as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(result.ViewBag.Items, Is.Not.Null);
        }
    }
}
