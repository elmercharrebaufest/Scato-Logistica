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
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ExistePedidoDeTrasladoControllerTest
    {
        private ExistePedidoDeTrasladoController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioActividadFactory<IExistePedidoDeTrasladoService>> factory;
        private Mock<IExistePedidoDeTrasladoService> contract;
        private NullLogger log;
        private ExistePedidoDeTrasladoDto model;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            factory = new Mock<IServicioActividadFactory<IExistePedidoDeTrasladoService>>();
            contract = new Mock<IExistePedidoDeTrasladoService>();
            log = new NullLogger();
            model = new ExistePedidoDeTrasladoDto{WorkflowCodigo = "W", WorkflowId = Guid.NewGuid()};
            target = new ExistePedidoDeTrasladoController(log,factory.Object, servRepositorio.Object);
            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                               {
                                   WorkflowDefinicionId = 1,
                                   WorkflowCodigo = "W",
                                   WorkflowId = 1
                               });
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(s => s.ExistePedidoDeTraslado(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());
        }

        [Test]
        public void IndexControlRecorridoNull()
        {
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(((ExistePedidoDeTrasladoDto)resultado.Model).WorkflowCodigo,"W");
        }

        [Test]
        public void IndexControlRecorridoNotNull()
        {
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto{Comentario = "C"});
            var resultado = target.Index(Guid.NewGuid()) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
            Assert.AreEqual(((ExistePedidoDeTrasladoDto)resultado.Model).WorkflowCodigo, "W");
            Assert.AreEqual(resultado.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "C");

        }

        [Test]
        public void Cancelar()
        {
            var resultado = target.Cancelar() as RedirectToRouteResult;

            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void ReintentarModeloValidoSinErrores()
        {
            var resultado = target.Reintentar(model, 1, new DatosUsuario {NombreUsuario = "wandino"}) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void ReintentarModeloInvalido()
        {
            target.ModelState.AddModelError("Error", new Exception());
            var resultado = target.Reintentar(model, 1, new DatosUsuario{NombreUsuario = "Wandino"}) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void ReintentarModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.ExistePedidoDeTraslado(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.Reintentar(model, 1, new DatosUsuario{NombreUsuario = "Wandino"}) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void RechazarModeloValidoSinErrores()
        {
            var resultado = target.Rechazar(model, 1, new DatosUsuario{NombreUsuario = "Wandino"}) as RedirectToRouteResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.RouteValues["action"], "Index");
            Assert.AreEqual(resultado.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void RechazarModeloinvalido()
        {
            target.ModelState.AddModelError("Error", new Exception());
            var resultado = target.Rechazar(model, 1, new DatosUsuario { NombreUsuario = "Wandino" }) as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void RechazarModeloValidoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");

            contract.Setup(
                s =>
                s.ExistePedidoDeTraslado(It.IsAny<Guid>(),It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);

            var result = target.Rechazar(model, 1,new DatosUsuario{NombreUsuario = "Wandino"}) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.First().ErrorMessage, "error");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }
    }
}
