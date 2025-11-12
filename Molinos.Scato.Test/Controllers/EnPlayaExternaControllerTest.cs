using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class EnPlayaExternaControllerTest
    {
        private EnPlayaExternaController target;
        private Mock<IServicioActividadFactory<IEjecutarService>> actFactoryMock;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IRecorridoWorkflow> recorridoWorkflowMock;
        private Mock<IServicioOrquestador> servOrquestadorMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IEjecutarService> contractMock;
        private DatosUsuario datos;

        [SetUp]
        public void SetUp()
        {
            actFactoryMock = new Mock<IServicioActividadFactory<IEjecutarService>>();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            recorridoWorkflowMock = new Mock<IRecorridoWorkflow>();
            servOrquestadorMock = new Mock<IServicioOrquestador>();
            contractMock = new Mock<IEjecutarService>();
            target = new EnPlayaExternaController(null, actFactoryMock.Object, servRepositorioMock.Object, recorridoWorkflowMock.Object, servOrquestadorMock.Object, servComandosMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1", PuestoDeTrabajoId = 1 };
        }


        [Test]
        public void TestIndex()
        {

            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Ejecutar(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());

            var result = target.Index(new ObservacionRDto { WorkflowInstanceId = Guid.NewGuid(), Observaciones = "", Id = 1 }, "", 0, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
