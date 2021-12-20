using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Moq;
using Molinos.Scato.Web.Controllers;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class LogHumedimetroControllerTest
    {

        private LogHumedimetroController target ;
        private Mock<IServicioRepositorio> servRepositorioMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            target = new LogHumedimetroController(new NullLogger(), servRepositorioMock.Object);
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(c => c.ListarCentros())
                               .Returns(new List<CentroDto> {new CentroDto {Descripcion = "a",Id = 1}});
            servRepositorioMock.Setup(h => h.ListarHumedimetros())
                               .Returns(new List<HumedimetroDto> {new HumedimetroDto {Descripcion = "H", Id = 1}});

            var result = target.Index();
            var centro = target.ViewBag.Centros;
            var e = (List<SelectListItem>) centro;
            var todos = e.ElementAt(0);
            var centroA = e.ElementAt(1);
            Assert.IsNotNull(result);
            Assert.IsTrue(todos.Text == "Todos");
            Assert.IsTrue(centroA.Text == "a");
            
        }
        
        [Test]
        public void TestLogHumedimetroController()
        {
            servRepositorioMock.Setup(c => c.ListarCentros())
                               .Returns(new List<CentroDto> {new CentroDto {Descripcion = "a", Id = 1}});
            servRepositorioMock.Setup(h => h.ListarHumedimetrosPorCentro(1))
                               .Returns(new List<HumedimetroDto> {new HumedimetroDto {Descripcion = "H", Id = 1}});
            servRepositorioMock.Setup(
                l => l.ListarListadoDeMuestrasDeHumedad(1, 1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                               .Returns(new List<MuestraDeHumedadDto>
                                   {
                                       new MuestraDeHumedadDto {Centro = "a", Humedimetro = "H", Usuario = "W", Modalidad = "M", NumeroDocumentoIngreso = "1234", NumeroOrden = "1234"}
                                   });
            var result = target.Index(new LogHumedimetroDto{CentroId =  1, FechaDesde = It.IsAny<DateTime>(), HumedimetroId = 1,FechaHasta = It.IsAny<DateTime>()});
            servRepositorioMock.Verify(p => p.ListarListadoDeMuestrasDeHumedad(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once()); 
            
        }
    }
}
