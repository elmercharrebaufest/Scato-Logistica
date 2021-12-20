using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
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
    public class IngresoCIUControllerTest
    {
        private IngresoCIUController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IIngresoCIUService>> actFactoryMock;
        private Mock<IIngresoCIUService> contractMock;
        private Mock<IListaDeWorkflows> listaMock;
        private NullLogger logger;
        private DatosUsuario datos;
        private IngresoCIUModel model;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresoCIUService>>();
            contractMock = new Mock<IIngresoCIUService>();
            logger = new NullLogger();
            listaMock = new Mock<IListaDeWorkflows>();
            target = new IngresoCIUController(logger, actFactoryMock.Object, servRepositorioMock.Object, listaMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1" };

            recorrido = new RecorridoDto
            {
                PesoBruto = 555,
                PesoTara = 222,
                PesoBrutoFecha = new DateTime(2010, 2, 2),
                PesoTaraFecha = new DateTime(2010, 1, 1),
                Vehiculo = new VehiculoDto { Patente = "AAA111" },
                FechaEgreso = new DateTime(2012, 2, 2),
                Terminado = true,
                WorkflowDefinicionId = 5,
                Workflow = new WorkflowDto()
            };


            model = new IngresoCIUModel
            {
                Numero = "100000",
                Workflow = "CodigoBaufest",
                WorkflowDefinicionId = 5
            };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(It.IsAny<Guid>())).Returns(new DatosDeInstanciaDto{WorkflowDefinicionId = 5});
            var result = target.Index(It.IsAny<Guid>()) as ViewResult;
            var model = (IngresoCIUModel)result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(model.WorkflowDefinicionId, Is.EqualTo(5));
        }

        [Test]
        public void TestIndexPost()
        {
            servRepositorioMock.Setup(s => s.EsCiuAnulado(It.IsAny<string>())).Returns(false);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorNumeroCiu(It.IsAny<string>())).Returns((RecorridoDto)null);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            contractMock.Setup(s => s.IngresoCIU(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new Resultado());
            var result = target.Index(model, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            contractMock.Verify(v => v.IngresoCIU(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()), Times.Once());
        }

        [Test]
        public void TestIndexCiuAnulado()
        {
            servRepositorioMock.Setup(s => s.EsCiuAnulado(It.IsAny<string>())).Returns(true);
            var result = target.Index(model, datos) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.IngresoCiu_CiuAnulado));
        }

        [Test]
        public void TestIndexCiuEnUso()
        {
            servRepositorioMock.Setup(s => s.EsCiuAnulado(It.IsAny<string>())).Returns(false);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            listaMock.Setup(s => s.VerificarExistenciaDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoGuidPorNumeroCiu(It.IsAny<string>())).Returns(new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"));
            var result = target.Index(model, datos) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.IngresoCiu_CiuEnUso));
        }
    }
}
