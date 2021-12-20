using System;
using System.Web.Mvc;
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
    public class LibroMovimientosExistenciaGranosControllerTest
    {
        private LibroMovimientosExistenciaGranosController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private LibroMovimientosExistenciaGranosDto dto;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new LibroMovimientosExistenciaGranosController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            dto = new LibroMovimientosExistenciaGranosDto
                {
                    Id = 1,
                    FechaDesde = DateTime.Today.AddDays(-7),
                    FechaHasta = DateTime.Today,
                    MaterialId = 1,
                    MaterialDesc = "Material 1"
                };
        }

        [Test]
        public void TestIndex()
        {
            //servRepositorioMock.Setup(s => s.ObtenerStock(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(2000);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirLibroMovimientosExistenciaGranos>())).Returns(new Resultado());

            var datosUsuario = new DatosUsuario {CentroId = 1};
            var result = target.Imprimir(datosUsuario, dto) as ActionResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1)); 
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.ImpresionEnviada));
        }
    }
}
