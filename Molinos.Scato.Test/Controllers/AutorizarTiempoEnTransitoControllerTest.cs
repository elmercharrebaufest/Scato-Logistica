using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    public class AutorizarTiempoEnTransitoControllerTest
    {
        private AutorizarTiempoEnTransitoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAutorizarTiempoEnTransitoService>> actFactoryMock;
        private Mock<IAutorizarTiempoEnTransitoService> contractMock;
        private AutorizacionTiempoEnTransitoDto dto;
        private Guid guid;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAutorizarTiempoEnTransitoService>>();
            target = new AutorizarTiempoEnTransitoController(null, actFactoryMock.Object, servRepositorioMock.Object);
            contractMock = new Mock<IAutorizarTiempoEnTransitoService>();

            dto = new AutorizacionTiempoEnTransitoDto()
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                NombreUsuario = "Usuario",
                Decision = false,
                Mensaje = "Mensaje"
            };
            guid = new Guid("11111111111111111111111111111111");
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(guid))
                .Returns(new DatosDeInstanciaDto { WorkflowDefinicionId = 1, WorkflowCodigo = "WFTest" });
            
            var control = new ControlDeTiempoDto()
            {
                ActividadDesde = "Pesada",
                ActividadHasta = "Pesada",
                CodigoControl = "AAA",
                TiempoMaximo = 500,
                WorkflowCodigo = "1",
                WorkflowId = 1
            };
            var log = new LogActividadDto()
            {
                Actividad = "Pesada",
                Fecha = DateTime.Now,
                WorkflowInstanceId = guid
            };

            servRepositorioMock.Setup(s => s.ObtenerUltimoLogActividad(It.IsAny<Guid>(), It.IsAny<string>())).Returns(log);
            servRepositorioMock.Setup(x => x.ObtenerAutorizacionTiempoEnTransito(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(
                new AutorizacionTiempoEnTransitoDto
                {

                });
            var result = target.Index(guid, new DatosUsuario{NombreUsuario = "Usuario"}) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        //[Test]
        //public void TestControlTiempoAceptado()
        //{
        //    dto = new AutorizacionTiempoEnTransitoDto()
        //    {
        //        Id = 1,
        //        WorkflowInstanceId = new Guid(),
        //        NombreUsuario = "Usuario",
        //        Decision = false,
        //        Mensaje = "Mensaje"
        //    };
        //    actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<string>(), It.IsAny<int>())).Returns(contractMock.Object);
        //    contractMock.Setup(s => s.AutorizarTiempoEnTransito(dto, It.IsAny<Guid>())).Returns(new Resultado());

        //    var result = target.Index(dto, "EgresoMaterialNoProductivo", 1) as RedirectToRouteResult;
        //    Assert.NotNull(result);
        //    Assert.AreEqual("Index", result.RouteValues["action"]);
        //}
    }
}
