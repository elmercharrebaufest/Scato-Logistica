using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class SalidaDeCentroControllerTest
    {
        private SalidaDeCentroController target;
        private NullLogger log;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioActividadFactory<IEjecutarService>> factory;
        private Mock<IEjecutarService> contract;

        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servRepositorio = new Mock<IServicioRepositorio>();
            factory = new Mock<IServicioActividadFactory<IEjecutarService>>();
            contract = new Mock<IEjecutarService>();
            target = new SalidaDeCentroController(log, factory.Object, servRepositorio.Object);

            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                           {
                               WorkflowDefinicionId = 1,
                               WorkflowCodigo = "W",
                               WorkflowId = 1
                           });

            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(s => s.Ejecutar(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());
        }

        [Test]
        public void IndexTest()
        {
            var datos = new DatosUsuario { CentroDescripcion = "centro 1", PuestoDeTrabajoId = 1 };

            var result =
                target.Index(new ObservacionRDto { WorkflowInstanceId = Guid.NewGuid(), Observaciones = "", Id = 1 }, "", 0, datos) as
                RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }
    }
}
