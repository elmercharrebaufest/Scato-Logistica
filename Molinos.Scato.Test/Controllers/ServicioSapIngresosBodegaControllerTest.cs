using System;
using System.Linq;
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
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ServicioSapIngresosBodegaControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private Mock<IServicioActividadFactory<IServicioSapIngresosBodegaService>> factory;
        private Mock<IServicioSapIngresosBodegaService> contract;
        private NullLogger log;
        private ServicioSapIngresosBodegaController target;
        private RecorridoDto recorrido;
        private ControlRecorridoDto controlRecorrido;
        private ServicioASapDto servicioASapDto;
        private DatosUsuario datosUsuario; 

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            factory = new Mock<IServicioActividadFactory<IServicioSapIngresosBodegaService>>();
            log = new NullLogger();
            contract = new Mock<IServicioSapIngresosBodegaService>();
            target = new ServicioSapIngresosBodegaController(log, factory.Object, servRepositorio.Object);

            recorrido = new RecorridoDto
                {
                    Id = 1,
                    Centro = new CentroDto {Id = 1, Descripcion = "C"},
                    Chofer = new ChoferDto {Nombre = "Chofer", Apellido = "Chofer", Id = 1},
                    InstanciaWorkflow = Guid.NewGuid(),
                    WorkflowDefinicionId = 1,
                    Workflow = new WorkflowDto {Id = 1,Activo = true, CentroId = 1, Codigo = "w", TipoDeWorkflow = TipoDeWorkflow.Egreso}
                };

            controlRecorrido = new ControlRecorridoDto
                {
                    ActividadXaml = "A",
                    Id = 1,
                    NombreUsuario = "Wandino",
                    WorkflowInstanceId = recorrido.InstanciaWorkflow,
                    Comentario = "."
                };

            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                               {
                                   WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                                   WorkflowCodigo = recorrido.Workflow.Codigo,
                                   WorkflowId = recorrido.Workflow.Id
                                   
                               });

            servicioASapDto = new ServicioASapDto
                {
                    WorkflowCodigo = "W",
                    WorkflowId = recorrido.InstanciaWorkflow
                };

            datosUsuario = new DatosUsuario
                {
                    CentroId = 1,
                    CentroDescripcion = "C",
                    NombrePc = "BF",
                    NombreUsuario = "Wandino"
                };

            factory.Setup(f => f.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
        }

        [Test]
        public void IndexControlRecorridoNotNullTest()
        {
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(controlRecorrido);
            var result = target.Index(recorrido.InstanciaWorkflow) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState["WorkflowId"].Errors.First().ErrorMessage, ".");
        }


        [Test]
        public void IndexControlRecorridoNullTest()
        {
            var result = target.Index(recorrido.InstanciaWorkflow) as ViewResult;

            Assert.NotNull(result);
            Assert.True(result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void CancelarTest()
        {
            var result = target.Cancelar() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);

        }

        [Test]
        public void EncolarErrorTest()
        {
            ResultadoCrear r = new ResultadoCrear();
            r.Error("Error", "Error");

            contract.Setup(
                c =>
                c.ServicioSapIngresosBodega(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(r);

            var result = target.Encolar(servicioASapDto, recorrido.WorkflowDefinicionId, datosUsuario) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState["Error"].Errors.First().ErrorMessage, "Error");
        }

        [Test]
        public void EncolarTest()
        {
            contract.Setup(
                c =>
                c.ServicioSapIngresosBodega(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new ResultadoCrear());

            var result =
                target.Encolar(servicioASapDto, recorrido.WorkflowDefinicionId, datosUsuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index",result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);


        }

        [Test]
        public void ReintentarErrorTest()
        {
            ResultadoCrear r = new ResultadoCrear();
            r.Error("Error", "Error");

            contract.Setup(
                c =>
                c.ServicioSapIngresosBodega(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(r);

            var result = target.Encolar(servicioASapDto, recorrido.WorkflowDefinicionId, datosUsuario) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState["Error"].Errors.First().ErrorMessage, "Error");
        }

        [Test]
        public void ReintentarTest()
        {
            contract.Setup(
                c =>
                c.ServicioSapIngresosBodega(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new ResultadoCrear());

            var result =
                target.Encolar(servicioASapDto, recorrido.WorkflowDefinicionId, datosUsuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }
    }
}
