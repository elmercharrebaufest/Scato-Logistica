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
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class BajaCTGDefinitivoControllerTest
    {
        private BajaCTGDefinitivoController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioActividadFactory<IBajaCTGDefinitivoService>> factory;
        private Mock<IBajaCTGDefinitivoService> contract;
        private NullLogger log;
        private DatosUsuario usuario;
        private BajaCTGDto model;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            contract = new Mock<IBajaCTGDefinitivoService>();
            factory = new Mock<IServicioActividadFactory<IBajaCTGDefinitivoService>>();
            log = new NullLogger();
            usuario = new DatosUsuario { NombreUsuario = "wandino" };
            model = new BajaCTGDto { WorkflowId = Guid.NewGuid(), WorkflowCodigo = "W", CodigoDeBaja = "C" };
            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaAltaCTGPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaAltaCTGDto
                           {
                               WorkflowDefinicionId = 1,
                               SolicitaConfirmarCTG = true,
                               WorkflowCodigo = "W"
                           });

            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s =>
                s.BajaCTGDefinitivo(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());
            target = new BajaCTGDefinitivoController(log, factory.Object, servRepositorio.Object);

        }

        [Test]
        public void IndexControlRecorridoNull()
        {
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
        }

        [Test]
        public void IndexControlRecorridoNotNull()
        {
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto { Comentario = "C" });
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.AreEqual(resultado.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "C");
        }

        [Test]
        public void Cancelar()
        {
            var resultado = target.Cancelar() as RedirectToRouteResult;
            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void CargarBajaCtgSinErrores()
        {
            var resultado = target.CargarBajaCtg(model, 1, true, usuario) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void CargarBajaCtgConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.BajaCTGDefinitivo(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.CargarBajaCtg(model, 1, true, usuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
        }

        [Test]
        public void ReintentarCtgSinErrores()
        {
            var resultado = target.ReintentarCtg(model, 1, true, usuario) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void ReintentarCtgConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.BajaCTGDefinitivo(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.ReintentarCtg(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
        }
    }
}
