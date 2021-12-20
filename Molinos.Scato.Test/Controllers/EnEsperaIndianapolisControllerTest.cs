using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    public class EnEsperaIndianapolisControllerTest
    {
        private EnEsperaIndianapolisController target;
        private Mock<IServicioActividadFactory<IEjecutarService>> actFactoryMock;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IEjecutarService> contractMock;
        private DatosUsuario datos;

        [SetUp]
        public void SetUp()
        {
            actFactoryMock = new Mock<IServicioActividadFactory<IEjecutarService>>();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            contractMock = new Mock<IEjecutarService>();
            target = new EnEsperaIndianapolisController(null, actFactoryMock.Object, servRepositorioMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1", PuestoDeTrabajoId = 1};
        }


        [Test]
        public void TestIndex()
        {

            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Ejecutar(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());

            var result = target.Index(new Guid(), datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
