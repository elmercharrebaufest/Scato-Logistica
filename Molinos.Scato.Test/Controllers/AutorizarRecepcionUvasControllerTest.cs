using System;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
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
    public class AutorizarRecepcionUvasControllerTest
    {
        private AutorizarRecepcionUvasController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAutorizarRecepcionUvasService>> actFactoryMock;
        private Guid guid;
        private Mock<IAutorizarRecepcionUvasService> contractMock;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAutorizarRecepcionUvasService>>();
            target = new AutorizarRecepcionUvasController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);
            contractMock = new Mock<IAutorizarRecepcionUvasService>();
            guid = new Guid();
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(guid))
                .Returns(new RecorridoDto { Centro = new CentroDto { Id = 1 }, Patente = "TES123", Chofer = new ChoferDto { Id = 1 }, Material = new MaterialDto(){Id = 1, Descripcion = "Material2"}, Workflow = new WorkflowDto(){Id = 1, Descripcion = "WFTest", Codigo = "WFTest"}, WorkflowDefinicionId = 1, TipoComercial = new TipoComercialDto{Descripcion = "a"}});
            servRepositorioMock.Setup(s => s.ObtenerRemitoBodegaUvaPorGuid(guid))
                .Returns(new RemitoBodegaUvaDto{ Proveedor = "proveedor", Vinedo = "vinedo", VinedoId = 1});
            servRepositorioMock.Setup(s => s.ObtenerKilosARecibirPorVinedo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new VerificarKilosDeclaradosPorFincaDto());
            
            var result = target.Index(guid, new DatosUsuario{NombreUsuario = "Usuario"}) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestAutorizar()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.AutorizarRecepcionUvas(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>())).Returns(true);

            var viewResult = target.Index(new ControlRecorridoDto(), "workflow", 1) as ViewResult;

            contractMock.Verify(p => p.AutorizarRecepcionUvas(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()));
        }
    }
}
