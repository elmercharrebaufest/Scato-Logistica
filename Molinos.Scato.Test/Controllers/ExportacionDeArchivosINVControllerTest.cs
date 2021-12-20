using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
    public class ExportacionDeArchivosINVControllerTest
    {
        private ExportacionDeArchivosINVController target;
        private Mock<IServicioRepositorio> servRepositorioMock;


        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            target = new ExportacionDeArchivosINVController(new NullLogger(), servRepositorioMock.Object);
        }


        [Test]
        public void TestIndex()
        {
            var result = target.Index() as ViewResult;


            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestExportacionDeArchivosINV()
        {
            servRepositorioMock.Setup(s => s.ListarArchivoINV(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                 .Returns(new List<ArchivoINVFilaDto> { new ArchivoINVFilaDto {  } });

            var result = target.Index(new ExportacionDeArchivosINVDto
                    {
                        FechaDesde = DateTime.Now,
                        FechaHasta = DateTime.Now
                    },new DatosUsuario()) as ViewResult;
            servRepositorioMock.Verify(p => p.ListarArchivoINV(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Exactly(1)); 
        }
    }
}
