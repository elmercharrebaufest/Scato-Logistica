using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class IngresoNumeroCotControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private IngresoNumeroCotController target;
        private NullLogger log;
        private Mock<IServicioActividadFactory<IIngresoNumeroCotService>> factory;
        private Mock<IIngresoNumeroCotService> contract;
        private IngresoNumeroCotModel model;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            log = new NullLogger();
            factory = new Mock<IServicioActividadFactory<IIngresoNumeroCotService>>();
            contract = new Mock<IIngresoNumeroCotService>();
            model = new IngresoNumeroCotModel{InstanciaWorkflow = Guid.NewGuid(), Mensaje = "M",NumeroCot = "1234",WorkflowDefinicionId = 1};

            target = new IngresoNumeroCotController(log,factory.Object,servRepositorio.Object);

            servRepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                           .Returns(new DatosDeInstanciaDto
                               {
                                   WorkflowCodigo = "W",
                                   WorkflowDefinicionId = 1,
                                   WorkflowId = 1
                               });
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);

            contract.Setup(
                s => s.IngresoNumeroCot(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new Resultado());
        }

        [Test]
        public void IndexGetControlNull()
        {
            var instance = Guid.NewGuid();
            var result = target.Index(instance) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((IngresoNumeroCotModel)result.ViewData.Model).InstanciaWorkflow, instance);
            Assert.AreEqual(((IngresoNumeroCotModel)result.ViewData.Model).WorkflowDefinicionId, 1);
        }

        [Test]
        public void IndexGetControlNotNull()
        {
            var instance = Guid.NewGuid();
            servRepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(new ControlRecorridoDto{Comentario = "C"});
            var result = target.Index(instance) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((IngresoNumeroCotModel)result.ViewData.Model).InstanciaWorkflow, instance);
            Assert.AreEqual(((IngresoNumeroCotModel)result.ViewData.Model).WorkflowDefinicionId, 1);
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage,"C");
        }

        [Test]
        public void IndexPost()
        {
            var result = target.Index(model) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).InstanciaWorkflow, model.InstanciaWorkflow);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).WorkflowDefinicionId, 1);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).Mensaje, "M");
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).NumeroCot, "1234");
        }

        [Test]
        public void AceptarModeloValido()
        {
            var result = target.Aceptar(model) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName,"Confirmacion");
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).InstanciaWorkflow, model.InstanciaWorkflow);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).WorkflowDefinicionId, 1);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).Mensaje, "M");
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).NumeroCot, "1234");
        }

        [Test]
        public void AceptarModeloInvalido()
        {
            target.ModelState.AddModelError("Error","error");
            var result = target.Aceptar(model) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).InstanciaWorkflow, model.InstanciaWorkflow);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).WorkflowDefinicionId, 1);
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).Mensaje, "M");
            Assert.AreEqual(((IngresoNumeroCotModel)result.Model).NumeroCot, "1234");
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void ConfirmarResultadoExitoso()
        {
            var result = target.Confirmar(model, new DatosUsuario {NombreUsuario = "Wandino"}) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "OK"); 
        }

        [Test]
        public void ConfirmarResultadoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            contract.Setup(
                s => s.IngresoNumeroCot(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(resultado);
            var result = target.Confirmar(model, new DatosUsuario { NombreUsuario = "Wandino" }) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "ERROR");
            Assert.AreEqual(target.TempData["Alerta"], "Ha ocurrido un error al ingresar el Nro de COT");
            Assert.AreEqual(target.TempData["TipoAlerta"], TipoAlerta.Error);
        }
    }
}
