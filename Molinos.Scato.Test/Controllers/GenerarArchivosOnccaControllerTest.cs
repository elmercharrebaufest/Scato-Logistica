using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class GenerarArchivosOnccaControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComandos;
        private NullLogger log;
        private GenerarArchivosOnccaController target;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComandos = new Mock<IServicioComandos>();
            log = new NullLogger();

            servRepositorio.Setup(s => s.ListarCentros())
                           .Returns(new List<CentroDto> {new CentroDto {Id = 1, Descripcion = "C"}});
            
            target = new GenerarArchivosOnccaController(log, servRepositorio.Object, servComandos.Object);
        }

        [Test]
        public void IndexTest()
        {
            

            var result = target.Index() as ViewResult;
            
            Assert.NotNull(result);
            Assert.That(((List<SelectListItem>)(result.ViewBag.TiposArchivo)).Select(x=>x.Value), Is.EquivalentTo(new List<string>{"MCPR","MCPE"}));
            Assert.AreEqual(((List<CentroDto>)(result.ViewData.Model)).ElementAt(0).Descripcion, "C");
            
        }
        [Test]
        public void GenerarSinCentroTest()
        {
            var result = target.Generar(null, "a", new DateTime(2015, 06, 1), new DateTime(2015, 06, 01)) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState[""].Errors.Count, 1);
        }

        [Test]
        public void GenerarSinFechaInicioTest()
        {
            var result = target.Generar(new List<int>{1}, "a", null, new DateTime(2015, 06, 01)) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState["fechaInicio"].Errors.Count, 1);
        }

        [Test]
        public void GenerarSinFechaFinTest()
        {
            var result = target.Generar(new List<int> { 1 }, "a",new DateTime(2015, 06, 01),  null) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState["fechaFin"].Errors.Count, 1);
        }

        [Test]
        public void GenerarArchivoSinContenidoTest()
        {
            servRepositorio.Setup(
                s =>
                s.ListarArchivoOncca(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                                     It.IsAny<TipoDeWorkflow>())).Returns(new List<OnccaEmitidasDto>());
            var result = target.Generar(new List<int> {1}, "MCPE", new DateTime(2009, 01, 01),
                                        new DateTime(2010, 01, 01)) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(result.ViewData.ModelState[""].Errors.First().ErrorMessage, "No existen datos para el/los centro/s seleccionados");
        }

        [Test]
        public void GenerarArchivoTest()
        {

            servRepositorio.Setup(
                s =>
                s.ListarArchivoOncca(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                                     It.IsAny<TipoDeWorkflow>())).Returns(new List<OnccaEmitidasDto>{new OnccaEmitidasDto{AcopladoPatente = "AAA111",Contrato = "A1"}});
            var result = target.Generar(new List<int> { 1 }, "MCPE", new DateTime(2009, 01, 01),
                                        new DateTime(2010, 01, 01));

            Assert.NotNull(result);
        }
    }
}
