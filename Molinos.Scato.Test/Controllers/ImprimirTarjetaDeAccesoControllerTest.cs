using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ImprimirTarjetaDeAccesoControllerTest
    {
        private ImprimirTarjetaDeAccesoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private ImpresionTarjetaDeAccesoModel impresionModel;
        private Mock<IListaDeWorkflows> listaMock;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            listaMock = new Mock<IListaDeWorkflows>();
            target = new ImprimirTarjetaDeAccesoController(
                null, servRepositorioMock.Object, servComandosMock.Object, listaMock.Object);

            impresionModel = new ImpresionTarjetaDeAccesoModel
                {
                    Numero = "0001",
                };
        }
        [Test]
        public void TestIndex()
        {

            var result = target.Index() as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
        [Test]
        public void TestExitoso()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            var result = target.Index(impresionModel, new DatosUsuario()) as RedirectToRouteResult; //as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.ImpresionEnviada));
            //Assert.AreEqual("Index", result.RouteValues["action"]);
            //Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }
        [Test]
        public void TestError()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(resultado);
            var result = target.Index(impresionModel, new DatosUsuario()) as ViewResult; //as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }
        [Test]
        public void TestTarjetaInvalida()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            var result = target.Index(impresionModel, new DatosUsuario()) as ViewResult; //as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>()), Times.Exactly(0));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ImpresionTarjetaDeAcceso_Invalido));
        }
        [Test]
        public void TestTarjetaBloqueada()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            var result = target.Index(impresionModel, new DatosUsuario()) as ViewResult; //as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>()), Times.Exactly(0));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ImpresionTarjetaDeAcceso_Bloqueado));
        }
        [Test]
        public void TestTarjetaEnUso()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.VerificarExistenciaDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            var result = target.Index(impresionModel, new DatosUsuario()) as ViewResult; //as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>()), Times.Exactly(0));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ImpresionTarjetaDeAcceso_EnUso));
        }
    }
}
