using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
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
    public class DistribucionDeAlmacenesControllerTest
    {
        private DistribucionDeAlmacenesController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IDistribucionDeAlmacenesService>> actFactoryMock;
        private Mock<IDistribucionDeAlmacenesService> contractMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IDistribucionDeAlmacenesService>>();
            contractMock = new Mock<IDistribucionDeAlmacenesService>();
            target = new DistribucionDeAlmacenesController(new NullLogger(),  servRepositorioMock.Object, actFactoryMock.Object);
        }


        [Test]
        public void TestIndex()
        {
            var recorridoDto = new RecorridoDto
            {
                Centro = new CentroDto{Descripcion = "centro"},
                InstanciaWorkflow = new Guid(),
                Patente = "aaa111",
                PesoNetoBodegaEnLitros = 10,
                Workflow = new WorkflowDto { Codigo = "Workflow 1", Descripcion = "Worfklow desc", TipoDeWorkflow = TipoDeWorkflow.Egreso},
                Material = new MaterialDto{Id = 1}
            };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);


            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<AlmacenDto>());

            var result = target.Index(new DatosUsuario{NombreUsuario = "Usuario"},new Guid()) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestDistribucionDeAlmacenes()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.DistribucionDeAlmacenes(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<DistribucionDeAlmacenesDto>())).Returns(new Resultado());
            var model =  new DistribucionDeAlmacenesDto{DistribucionesDeAlmacenesJson = new List<DistribucionDeAlmacenDto>().ToJson()};

            var result = target.Index(new DatosUsuario(), model) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
