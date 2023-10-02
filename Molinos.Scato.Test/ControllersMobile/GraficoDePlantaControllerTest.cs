using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.WebMobile.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.ControllersMobile
{
    [TestFixture]
    public class GraficoDePlantaControllerTest
    {
        private GraficoDePlantaController target;
        private Mock<IConfiguracionProvider> configuracionProvider;
        private Mock<IFirmaProvider> firmaProvider;
        private Mock<IServicioRepositorio> servicio;
        private Mock<IServicioComandos> comandos;
        private Mock<IListaDeWorkflows> listaDeWorkflows;

        private NullLogger log;
        
        [SetUp]
        public void SetUp()
        {
            configuracionProvider = new Mock<IConfiguracionProvider>();
            listaDeWorkflows = new Mock<IListaDeWorkflows>();
            servicio = new Mock<IServicioRepositorio>();
            comandos = new Mock<IServicioComandos>();
            firmaProvider = new Mock<IFirmaProvider>();
            listaDeWorkflows = new Mock<IListaDeWorkflows>();
            log = new NullLogger();
            target = new GraficoDePlantaController(log, servicio.Object, firmaProvider.Object, configuracionProvider.Object, comandos.Object, listaDeWorkflows.Object);
        }

        [Test]
        public void IndexTest()
        {
            var materialIdYDescripcion = new MaterialIdYDescripcionDto()
            {
                MaterialId = 1,
                Descripcion = "Soja"
            };
            servicio.Setup(x => x.ObtenerMaterialIdYDescripcionPorCodigoSap(It.IsAny<string>())).Returns(materialIdYDescripcion);
            configuracionProvider.Setup(x => x.AppSettings.Get(It.IsAny<string>())).Returns("1");

            servicio.Setup(x => x.ListarEstadoCupos(5)).Returns(new List<CupoMobileDto>());
            servicio.Setup(x => x.ListarEstadoPlanta(5, true, true)).Returns(new List<EstadoMaterialDto>());
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;


            var resultado = target.Index() as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewName, "");
            Assert.AreEqual((int)resultado.ViewBag.MaterialId, 1);
            Assert.AreEqual((string)resultado.ViewBag.MaterialDescripcion, "Soja");
        }

        [Test]
        public void IndexMaterialYDescripcionNullTest()
        {
            servicio.Setup(x => x.ObtenerMaterialIdYDescripcionPorCodigoSap(It.IsAny<string>()));
            configuracionProvider.Setup(x => x.AppSettings.Get(It.IsAny<string>())).Returns("1");

            servicio.Setup(x => x.ListarEstadoPlanta(5, true, true)).Returns(new List<EstadoMaterialDto>());
            servicio.Setup(x => x.ListarEstadoCupos(5)).Returns(new List<CupoMobileDto>());
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;


            var resultado = target.Index() as ViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewName, "");
            Assert.IsNull(resultado.ViewBag.MaterialId);
            Assert.IsNull(resultado.ViewBag.MaterialDescripcion);
        }

        [Test]
        public void MenuTest()
        {
            var resultado = target.Menu() as PartialViewResult;

            Assert.NotNull(resultado);
            Assert.AreEqual(resultado.ViewName, "_Menu");
        }

        [Test]
        public void LogoTest()
        {
            var resultado = target.Logo();

            Assert.NotNull(resultado);
            Assert.That(resultado, Is.TypeOf<FileContentResult>());
            Assert.That(resultado.FileContents, Is.TypeOf<Byte[]>());
            Assert.That(resultado.ContentType, Is.EqualTo("image/png"));
        }

        [Test]
        public void GenerarGraficoCamionesPorDia()
        {
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;
            var model = new GraficoCamionesDiaDto
            {
                MaterialDescripcion = "material1",
                MaterialId = 33
            };
            servicio.Setup(x => x.ObtenerGraficoDeCamionesPorDia(It.IsAny<GraficoCamionesDiaDto>(), It.IsAny<int>())).Returns(model);
            configuracionProvider.Setup(x => x.AppSettings.Get(It.IsAny<string>()));
            var resultado = target.GenerarGraficoCamionesPorDia(new GraficoCamionesDiaDto()) as JsonResult;

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.That(resultado.Data, Is.TypeOf<GraficoCamionesDiaDto>());
            Assert.AreEqual(((GraficoCamionesDiaDto)resultado.Data).MaterialId, 33);
            Assert.AreEqual(((GraficoCamionesDiaDto)resultado.Data).MaterialDescripcion, "material1");
        }

        [Test]
        public void GenerarGraficoCamionesPorHora()
        {
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;

            var model = new GraficoCamionesHoraDto
            {
                MaterialDescripcion = "material1",
                MaterialId = 33
            };
            servicio.Setup(x => x.ObtenerGraficoDeCamionesPorHora(It.IsAny<GraficoCamionesHoraDto>(), It.IsAny<int>())).Returns(model);
            configuracionProvider.Setup(x => x.AppSettings.Get(It.IsAny<string>()));
            var resultado = target.GenerarGraficoCamionesPorHora(new GraficoCamionesHoraDto()) as JsonResult;

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.That(resultado.Data, Is.TypeOf<GraficoCamionesHoraDto>());
            Assert.AreEqual(((GraficoCamionesHoraDto)resultado.Data).MaterialId, 33);
            Assert.AreEqual(((GraficoCamionesHoraDto)resultado.Data).MaterialDescripcion, "material1");
        }

        [Test]
        public void GenerarGraficoEficienciaHidraulicas()
        {
            var claimCollection = new List<Claim>
            {
                new Claim("CentroId", "5")
            };
            var identity = new GenericIdentity("TestRun");
            identity.AddClaims(claimCollection);
            var principal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = principal;

            var model = new GraficoEficienciaHidraulicasDto
            {
                MaterialDescripcion = "material1",
                MaterialId = 33
            };
            servicio.Setup(x => x.ObtenerGraficoDeEficienciaHidraulicas(It.IsAny<GraficoEficienciaHidraulicasDto>(), It.IsAny<int>(), It.IsAny<string>())).Returns(model);
            configuracionProvider.Setup(x => x.AppSettings.Get(It.IsAny<string>()));
            var resultado = target.GenerarGraficoEficienciaHidraulicas(new GraficoEficienciaHidraulicasDto()) as JsonResult;

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.That(resultado.Data, Is.TypeOf<GraficoEficienciaHidraulicasDto>());
            Assert.AreEqual(((GraficoEficienciaHidraulicasDto)resultado.Data).MaterialId, 33);
            Assert.AreEqual(((GraficoEficienciaHidraulicasDto)resultado.Data).MaterialDescripcion, "material1");
        }
    }
}
