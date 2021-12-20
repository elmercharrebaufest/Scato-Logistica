using System;
using System.Collections.Generic;
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class IngresoDeObservacionesControllerTest
    {
        private IngresoDeObservacionesController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IIngresoDeObservacionesService>> actFactoryMock;
        private Mock<IIngresoDeObservacionesService> contractMock;
        private ObservacionDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresoDeObservacionesService>>();
            contractMock = new Mock<IIngresoDeObservacionesService>();
            target = new IngresoDeObservacionesController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

            dto = new ObservacionDto
            {
                WorkflowInstanceId = new Guid(),
                Observaciones = "Observaciones 1"
            };
        }


        [Test]
        public void TestIndex()
        {
            var recorridoDto = new RecorridoDto
            {
                Patente = "AAA111",
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "123456789",
                Workflow = new WorkflowDto { Codigo = "Workflow 1" }
            };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ObtenerObservacion(It.IsAny<Guid>())).Returns(dto);

            var result = target.Index(new Guid()) as ViewResult;
            var workflow = (string)target.ViewBag.Workflow;
            var patente = (string)target.ViewBag.Patente;
            var tipoDocumento = (TipoDocumentoIngreso)target.ViewBag.TipoDocumentoIngreso;
            var numeroDocumento = (string)target.ViewBag.NumeroDocumentoIngreso;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(workflow, Is.EqualTo("Workflow 1"));
            Assert.That(patente, Is.EqualTo("AAA111"));
            Assert.That(tipoDocumento, Is.EqualTo(TipoDocumentoIngreso.CartaPorte));
            Assert.That(numeroDocumento, Is.EqualTo("123456789"));
        }

        [Test]
        public void TestIngresoDeObservaciones()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.IngresoDeObservaciones(It.IsAny<ObservacionDto>(), It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());

            var result = target.Index(dto, "Workflow",1, new DatosUsuario{PuestoDeTrabajoId = 1}) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestCargarPrecintosInvalidos()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("Key", "Error"));
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.IngresoDeObservaciones(It.IsAny<ObservacionDto>(), It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);

            var result = target.Index(dto, "Workflow",1, new DatosUsuario{PuestoDeTrabajoId = 1}) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
