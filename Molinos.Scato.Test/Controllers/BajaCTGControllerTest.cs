using System;
using System.Collections.Generic;
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
    public class BajaCTGControllerTest
    {
        private BajaCTGController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioActividadFactory<IBajaCTGService>> factory;
        private Mock<IBajaCTGService> contract;
        private NullLogger log;
        private DatosUsuario usuario;
        private BajaCTGDto model;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            contract = new Mock<IBajaCTGService>();
            factory = new Mock<IServicioActividadFactory<IBajaCTGService>>();
            log = new NullLogger();
            usuario = new DatosUsuario { NombreUsuario = "wandino" };
            model = new BajaCTGDto { WorkflowId = Guid.NewGuid() , WorkflowCodigo = "W", CodigoDeBaja = "C"};
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
                s.BajaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());
            target = new BajaCTGController(log, factory.Object, servRepositorio.Object, servcomando.Object);

        }

        [Test]
        public void IndexControlRecorridoNull()
        {
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.False(resultado.ViewBag.PostDeManual);
        }

        [Test]
        public void IndexControlRecorridoNotNull()
        {
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto { Comentario = "C" });
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.False(resultado.ViewBag.PostDeManual);
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
        public void CargarBajaCtgModeloValidoSinErrorres()
        {
            var resultado = target.CargarBajaCtg(model, 1, true, usuario) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void CargarBajaCtgModeloInvalido()
        {
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            target.ModelState.AddModelError("Error", new Exception());
            var resultado = target.CargarBajaCtg(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.True(resultado.ViewBag.PostDeManual);
        }

        [Test]
        public void CargarBajaCtgModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            contract.Setup(
                s =>
                s.BajaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.CargarBajaCtg(model, 1, true, usuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
            Assert.True(result.ViewBag.PostDeManual);
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
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.BajaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.ReintentarCtg(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
            Assert.False(result.ViewBag.PostDeManual);
        }

        [Test]
        public void RechazarSinErrores()
        {
            var resultado = target.Rechazar(model, 1, true, usuario) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void RechazarConErrores()
        {
            servRepositorio.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.BajaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.Rechazar(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
            Assert.True(result.ViewBag.PostDeManual);
        }
    }
}
