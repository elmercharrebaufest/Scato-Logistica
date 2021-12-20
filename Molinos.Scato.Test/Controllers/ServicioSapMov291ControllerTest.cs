using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ServicioSapMov291ControllerTest
    {
        private ServicioSapMov291Controller target;
        private Mock<IServicioActividadFactory<IServicioSapMov291Service>> actSapMock;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioSapMov291Service> contractMock;
        private DatosUsuario user;
        private Guid instance;

        [SetUp]
        public void SetUp()
        {
            actSapMock = new Mock<IServicioActividadFactory<IServicioSapMov291Service>>();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            contractMock = new Mock<IServicioSapMov291Service>();
            target = new ServicioSapMov291Controller(null, actSapMock.Object, servRepositorioMock.Object);
            user = new DatosUsuario { CentroDescripcion = "centro 1", PuestoDeTrabajoId = 1};
            instance = Guid.NewGuid();
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>()))
                               .Returns(new DatosDeInstanciaDto {WorkflowCodigo = "A", WorkflowDefinicionId = 1});
            actSapMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(
                s => s.ServicioSapMov291(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                        .Returns(new Resultado());
        }


        [Test]
        public void TestIndexControlRecorridoNull()
        {
            servRepositorioMock.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>())).Returns((ControlRecorridoDto)null);
            
            var result = target.Index(instance) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).WorkflowCodigo, "A");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).WorkflowId, instance);
        }

        [Test]
        public void TestIndexControlRecorridoNotNull()
        {
            servRepositorioMock.Setup(s => s.ObtenerControlRecorrido(It.IsAny<Guid>(), It.IsAny<string>()))
                               .Returns(new ControlRecorridoDto {Comentario = "Error control recorrido"});

            var result = target.Index(instance) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).WorkflowCodigo, "A");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).WorkflowId, instance);
            ModelState error;
            Assert.True(target.ModelState.TryGetValue("WorkflowId", out error));
            Assert.NotNull(error);
            Assert.AreEqual(error.Errors[0].ErrorMessage, "Error control recorrido");
        }

        [Test]
        public void TestCancelar()
        {
            var result = target.Cancelar() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void EncolarModeloInvalido()
        {
            target.ModelState.AddModelError("E", "Modelo invalido");

            var result = target.Encolar(new EgresoSinFleteFazonesTransmisionASapDto{CentroId = "1"}, 1, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).CentroId,"1");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void EncolarError()
        {
            var error = new Resultado();
            error.Error("E", "Resultado error");
            contractMock.Setup(
                s => s.ServicioSapMov291(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                        .Returns(error);
            var result = target.Encolar(new EgresoSinFleteFazonesTransmisionASapDto { CentroId = "1" }, 1, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            ModelState res;
            target.ModelState.TryGetValue("E", out res);
            Assert.NotNull(res);
            Assert.AreEqual(res.Errors[0].ErrorMessage, "Resultado error");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).CentroId, "1");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void EncolarCorrecto()
        {
            var result = target.Encolar(new EgresoSinFleteFazonesTransmisionASapDto { CentroId = "1" }, 1, user) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }


        [Test]
        public void ReintentarModeloInvalido()
        {
            target.ModelState.AddModelError("E", "Modelo invalido");

            var result = target.Reintentar(new EgresoSinFleteFazonesTransmisionASapDto { CentroId = "1" }, 1, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).CentroId, "1");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void ReintentarError()
        {
            var error = new Resultado();
            error.Error("E", "Resultado error");
            contractMock.Setup(
                s => s.ServicioSapMov291(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>()))
                        .Returns(error);
            var result = target.Reintentar(new EgresoSinFleteFazonesTransmisionASapDto { CentroId = "1" }, 1, user) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            ModelState res;
            target.ModelState.TryGetValue("E", out res);
            Assert.NotNull(res);
            Assert.AreEqual(res.Errors[0].ErrorMessage, "Resultado error");
            Assert.AreEqual(((EgresoSinFleteFazonesTransmisionASapDto)result.Model).CentroId, "1");
            Assert.AreEqual(result.ViewBag.WorkflowDefinicionId, 1);
        }

        [Test]
        public void ReintentarCorrecto()
        {
            var result = target.Reintentar(new EgresoSinFleteFazonesTransmisionASapDto { CentroId = "1" }, 1, user) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }
    }
}
