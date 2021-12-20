using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    public class ValidarContratoControllerTest
    {

        private Mock<IServicioRepositorio> servrepositorio;
        private NullLogger log;
        private Mock<IServicioActividadFactory<IValidarContratoService>> actFactory;
        private Mock<IValidarContratoService> contractMock;
        
        private ValidarContratoController target;

        private DatosDeInstanciaDto datosDeInstancia;
        private ControlRecorridoDto controlRecorrido;
        private CartaPorteDto cartaPorte;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servrepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            actFactory = new Mock<IServicioActividadFactory<IValidarContratoService>>();
            target = new ValidarContratoController(log,actFactory.Object,servrepositorio.Object);
            contractMock = new Mock<IValidarContratoService>();

            datosDeInstancia = new DatosDeInstanciaDto
                {
                    WorkflowId = 1,
                    WorkflowCodigo = "W1",
                    WorkflowDefinicionId = 1
                };

            controlRecorrido = new ControlRecorridoDto
                {
                    ActividadXaml = "ValidarContrato",
                    Id = 1,
                    WorkflowInstanceId = Guid.NewGuid(),
                    Fecha = new DateTime(2015, 12, 05)
                };

            datosUsuario = new DatosUsuario
                {
                    CentroId = 1,
                    NombreUsuario = "wandino",
                    NombrePc = "BFBF11111"
                };

            cartaPorte = new CartaPorteDto
                {
                    AcuerdoMarco = "AM",
                    Id = 1
                };

        }

        [Test]
        public void TestIndexGetControlRecorridoNotNull()
        {
            servrepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(datosDeInstancia);
            servrepositorio.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns(controlRecorrido);
            servrepositorio.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns(cartaPorte);

            var result = target.Index(controlRecorrido.WorkflowInstanceId) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.Model, Is.InstanceOf(typeof(ValidarContratoModel)));
            
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.That(result.ViewData.ModelState.Values, Is.Not.Null);
            Assert.AreEqual(result.ViewData.ModelState[""].Errors.Count, 1);
        }

        [Test]
        public void TestIndexGetControlRecorridoNull()
        {
            servrepositorio.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(datosDeInstancia);
            servrepositorio.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns(cartaPorte);

            var result = target.Index(controlRecorrido.WorkflowInstanceId) as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.Model, Is.InstanceOf(typeof(ValidarContratoModel)));
            Assert.True(result.ViewData.ModelState.IsValid);
            Assert.That(result.ViewData.ModelState.Values, Is.Not.Null);
            Assert.IsNull(result.ViewData.ModelState[""]);
        }

        [Test]
        public void TestIndexPost()
        {
            ValidarContratoModel model = new ValidarContratoModel();
            var result = target.Index(model) as ViewResult;


            Assert.NotNull(result);
            Assert.That(result.Model, Is.InstanceOf(typeof(ValidarContratoModel)));
            Assert.True(result.ViewData.ModelState.IsValid);
            Assert.That(result.ViewData.ModelState.Values, Is.Not.Null);
            Assert.IsNull(result.ViewData.ModelState[""]);
        }

        [Test]
        public void AceptarTest()
        {
            ValidarContratoModel model = new ValidarContratoModel();
            actFactory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(
                s =>
                s.ValidarContrato(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>(),
                                  It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrear());
            
            var result = target.Aceptar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data,"OK");
            
        }

        [Test]
        public void AceptarResultadoConErroresTest()
        {
            ValidarContratoModel model = new ValidarContratoModel();
            var resultadoConErrores = new Resultado();
            resultadoConErrores.Errores.Add(new KeyValuePair<string, string>("","Error"));

            actFactory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(
                s =>
                s.ValidarContrato(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>(),
                                  It.IsAny<ControlRecorridoDto>())).Returns(resultadoConErrores);


            var result = target.Aceptar(model, datosUsuario) as ViewResult;

            Assert.NotNull(result);
            Assert.True(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(((ValidarContratoModel)(result.ViewData.Model)).Mensaje, "Error");
            Assert.AreEqual(result.ViewName, "Confirmacion");
        }

        [Test]
        public void AceptarExceptionTest()
        {
            ValidarContratoModel model = new ValidarContratoModel();
            
            actFactory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(
                s =>
                s.ValidarContrato(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>(),
                                  It.IsAny<ControlRecorridoDto>())).Throws(new Exception());


            var result = target.Aceptar(model, datosUsuario) as ViewResult;

            Assert.NotNull(result);
            Assert.True(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(((ValidarContratoModel)(result.ViewData.Model)).Mensaje, Textos.IngresarLote_ErrorSap);
            Assert.AreEqual(result.ViewName, "Confirmacion");

        }

        [Test]
        public void AceptarModelStateNOTValidTest()
        {
            target.ModelState.Add("testError", new ModelState());
            target.ModelState.AddModelError("testError", "test");
            ValidarContratoModel model = new ValidarContratoModel();

            var result = target.Aceptar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "ERROR");
            Assert.AreEqual(target.TempData["Alerta"], Textos.ValidarContrato_ContratoRequerido);
            Assert.AreEqual(target.TempData["TipoAlerta"], TipoAlerta.Error);
        }

        [Test]
        public void ConfirmacionTest()
        {
            ValidarContratoModel model = new ValidarContratoModel();
            actFactory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(
                s =>
                s.ValidarContrato(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>(),
                                  It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrear());

            var result = target.Confirmar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "OK");

        }
    }
}
