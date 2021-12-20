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
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class AltaCTGControllerTest
    {
        private AltaCTGController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioActividadFactory<IAltaCTGService>> factory;
        private Mock<IAltaCTGService> contract;
        private NullLogger log;
        private DatosUsuario usuario;
        private AltaCTGDto model;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            contract = new Mock<IAltaCTGService>();
            factory = new Mock<IServicioActividadFactory<IAltaCTGService>>();
            log = new NullLogger();
            usuario = new DatosUsuario {NombreUsuario = "wandino"};
            model = new AltaCTGDto { WorkflowId = Guid.NewGuid(), TarifaReferencia = 1, CodigoCTG = "A", Sucursal = "10", NroOrden = "00000001"  };
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
                s.AltaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<decimal>(),
                          It.IsAny<ControlRecorridoDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Resultado());
            target = new AltaCTGController(log, factory.Object,servRepositorio.Object);

        }

        [Test]
        public void IndexControlRecorridoNull()
        {
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.False(resultado.ViewBag.PostDeManual);
        }

        [Test]
        public void IndexControlRecorridoNotNull()
        {
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto{Comentario = "C"});
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
        public void CargarAltaCtgModeloValidoSinErrorres()
        {
            var model = new AltaCTGDto {WorkflowId = Guid.NewGuid(), TarifaReferencia = 1, CodigoCTG = "A"};
            var resultado = target.CargarAltaCtg(model, 1, true, usuario) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void CargarAltaCtgModeloInvalido()
        {
            target.ModelState.AddModelError("Error", new Exception());
            var resultado = target.CargarAltaCtg(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(resultado.ViewBag.VerReintentar);
            Assert.True(resultado.ViewBag.PostDeManual);
        }

        [Test]
        public void CargarAltaCtgModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");

            contract.Setup(
                s =>
                s.AltaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<decimal>(),
                          It.IsAny<ControlRecorridoDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns(resultado);

            var result = target.CargarAltaCtg(model, 1, true, usuario) as ViewResult;
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
            var resultado = new Resultado();
            resultado.Error("Error","error");

            contract.Setup(
                s =>
                s.AltaCTG(It.IsAny<Guid>(), It.IsAny<DecisionCtg>(), It.IsAny<string>(), It.IsAny<decimal>(),
                          It.IsAny<ControlRecorridoDto>(), It.IsAny<string>(), It.IsAny<string>())).Returns(resultado);

            var result = target.ReintentarCtg(model, 1, true, usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
            Assert.False(result.ViewBag.VerReintentar);
            Assert.False(result.ViewBag.PostDeManual);
        }
    }
}
