using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ServicioSapIngresosEgresosFazonesControllerTest
    {
        private ServicioSapIngresosEgresosFazonesController target;
        private Mock<IServicioActividadFactory<IServicioSapIngresosEgresosFazonesService>> factory;
        private Mock<IServicioSapIngresosEgresosFazonesService> contract;
        private Mock<IServicioRepositorio> servRepositorio;
        private NullLogger log;
        private ServicioASapDto model;
        private DatosUsuario usuario;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IServicioActividadFactory<IServicioSapIngresosEgresosFazonesService>>();
            contract = new Mock<IServicioSapIngresosEgresosFazonesService>();
            servRepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            usuario = new DatosUsuario
            {
                NombreUsuario = "w",
                CentroId = 1,
                NombrePc = "PC1"
            };

            model = new ServicioASapDto { WorkflowCodigo = "W", WorkflowId = Guid.NewGuid() };


            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto
                           {
                               ActividadXaml = "ServicioSapIngresosEgresosFazones",
                               Comentario = "C",
                               Id = 1,
                               WorkflowInstanceId = Guid.NewGuid(),
                               Fecha = new DateTime(2015, 6, 6)
                           });
            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                           {
                               WorkflowCodigo = "1",
                               WorkflowDefinicionId = 1,
                               WorkflowId = 1
                           });
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s => s.ServicioSapIngresosEgresosFazones(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());
            target = new ServicioSapIngresosEgresosFazonesController(log, factory.Object, servRepositorio.Object);

        }

        [Test]
        public void TestIndexControlRecorridoNull()
        {
            var result = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(result.ViewData.ModelState.Values.ElementAt(0).Errors.FirstOrDefault().ErrorMessage, "C");
        }

        [Test]
        public void TestIndexControlRecorridoNotNull()
        {
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns((ControlRecorridoDto)null);
            var result = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(result.ViewData.ModelState.Values.Count, 0);
        }

        [Test]
        public void TestCancelar()
        {
            var result = target.Cancelar() as RedirectToRouteResult;

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TestEncolarModeloValidoSinErrores()
        {
            var result = target.Encolar(model, 1, usuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TestEncolarModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "Error");

            contract.Setup(
                s => s.ServicioSapIngresosEgresosFazones(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.Encolar(model, 1, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Keys.ElementAt(0), "Error");
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void TestEncolarModeloInvalido()
        {
            target.ViewData.ModelState.AddModelError("Error", "Error");
            var result = target.Encolar(model, 1, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(target.ModelState.Keys.ElementAt(0), "Error");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void TestReintentarModeloValidoSinErrores()
        {
            var result = target.Reintentar(model, 1, usuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TestReintentarModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "Error");

            contract.Setup(
                s => s.ServicioSapIngresosEgresosFazones(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.Reintentar(model, 1, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Keys.ElementAt(0), "Error");
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void TestReintentarModeloInvalido()
        {
            target.ViewData.ModelState.AddModelError("Error", "Error");
            var result = target.Reintentar(model, 1, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(target.ModelState.Keys.ElementAt(0), "Error");
            Assert.AreEqual(target.ViewBag.WorkflowDefinicionId, 1);
        }
    }
}
