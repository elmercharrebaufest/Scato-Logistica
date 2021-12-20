using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class VerificacionSalidaFleteControllerTest
    {
        private Mock<IServicioActividadFactory<IVerificacionSalidaFleteService>> factory;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IVerificacionSalidaFleteService> contract;
        private VerificacionSalidaFleteController target;
        private NullLogger log;
        private RecorridoDto recorrido;
        private ControlRecorridoDto controlRecorrido;
        private DatosUsuario usuario;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IServicioActividadFactory<IVerificacionSalidaFleteService>>();
            contract = new Mock<IVerificacionSalidaFleteService>();
            servRepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            target = new VerificacionSalidaFleteController(log,factory.Object,servRepositorio.Object);
            usuario = new DatosUsuario
                {
                    CentroId = 1,
                    NombreUsuario = "w",
                    NombrePc = "w"
                };

            recorrido = new RecorridoDto
                {
                    Id = 1,
                    InstanciaWorkflow = Guid.NewGuid(),
                    Centro = new CentroDto {Id = 1, Descripcion = "C1"}
                };
            controlRecorrido = new ControlRecorridoDto
                {
                    ActividadXaml = "VerificacionSalidaFlete",
                    Fecha = new DateTime(2015, 6, 6),
                    Id = 1,
                    WorkflowInstanceId = recorrido.InstanciaWorkflow,
                    Comentario = "C"

                };
            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                               {
                                   WorkflowId = 1,
                                   WorkflowCodigo = "1",
                                   WorkflowDefinicionId = 1
                               });
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(controlRecorrido);
        }
        
        [Test]
        public void IndexControlRecorridoNoNuloTest()
        {
            var result = target.Index(recorrido.InstanciaWorkflow);

            Assert.NotNull(result);
            Assert.AreEqual(target.ViewBag.WorkflowId , recorrido.InstanciaWorkflow);
            Assert.AreEqual(target.ViewBag.WorkflowCodigo, "1");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(target.ViewBag.Mensaje,"C");

        }
        [Test]
        public void IndexControlRecorridoNuloTest()
        {
            
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns((ControlRecorridoDto) null);
            var result = target.Index(recorrido.InstanciaWorkflow);

            Assert.NotNull(result);
            Assert.AreEqual(target.ViewBag.WorkflowId , recorrido.InstanciaWorkflow);
            Assert.AreEqual(target.ViewBag.WorkflowCodigo, "1");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(target.ViewBag.Mensaje,Textos.VerificacionSalidaFlete_Error);
        }

        [Test]
        public void ReintentarTest()
        {
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s =>
                s.VerificacionSalidaFlete(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()));

            var result = target.Reintentar(recorrido.WorkflowDefinicionId, "1", recorrido.InstanciaWorkflow, usuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"],"ListaDeCamiones");
        }

        [Test]
        public void CancelarTest()
        {
            var result = target.Cancelar() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }
    }
}
