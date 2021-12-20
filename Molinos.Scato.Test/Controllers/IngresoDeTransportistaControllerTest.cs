using System;
using System.Collections.Generic;
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class IngresoDeTransportistaControllerTest
    {
        private IngresoDeTransportistaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IIngresoDeTransportistaService>> actFactoryMock;
        private Mock<IIngresoDeTransportistaService> contractMock;
        private TransportistaDto dto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresoDeTransportistaService>>();
            contractMock = new Mock<IIngresoDeTransportistaService>();
            target = new IngresoDeTransportistaController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);

            dto = new TransportistaDto
                {
                    Id = 1,
                    Cuit = "20-3485016-8",
                    RazonSocial = "a"
                };
        }


        [Test]
        public void TestIndex()
        {
            var recorridoDto = new RecorridoDto
            {
                Workflow = new WorkflowDto { Codigo = "Workflow 1" }
            };

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ListarProvincias()).Returns(new List<ProvinciaDto>());
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>())).Returns(new List<LocalidadDto>());
            servRepositorioMock.Setup(x => x.ObtenerFotoCPDeCartaDePortePorrecorrido(It.IsAny<Guid>())).Returns(new FotosDto { Fotos = new List<FotoDto>() });


            var result = target.Index(new Guid()) as ViewResult;
            var workflow = (string)target.ViewBag.Workflow;
            var workflowInstance = (Guid)target.ViewBag.WorkflowInstance;


            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(workflow, Is.EqualTo("Workflow 1"));
            Assert.That(workflowInstance, !Is.Null);
        }

        [Test]
        public void TestCrearPost()
        {
            var recorridoDto = new RecorridoDto
            {
                Workflow = new WorkflowDto { Codigo = "Workflow 1" }
            };
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.IngresoDeTransportista(It.IsAny<TransportistaDto>(), It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarRecorridoPorNumeroDocumentoYWorkflow(It.IsAny<string>(), It.IsAny<int>())).Returns(new List<RecorridoDto> { recorridoDto });

            var result = target.Index(dto, "Workflow",1, new Guid(), "", new DatosUsuario{PuestoDeTrabajoId = 1}) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

    }
}
