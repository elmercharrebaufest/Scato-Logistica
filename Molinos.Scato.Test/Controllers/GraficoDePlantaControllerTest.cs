using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class GraficoDePlantaControllerTest
    {
        private NullLogger log;
        private GraficoDePlantaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<EstadoPlantaDto> estados;
        private Mock<IListaDeWorkflows> listaWorkflowsMock;
        private Mock<IConfiguracionProvider> configuracion;
        private List<GraficoDePlantaDto> graficos;
        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            listaWorkflowsMock = new Mock<IListaDeWorkflows>();
            configuracion = new Mock<IConfiguracionProvider>();
            target = new GraficoDePlantaController(log, servRepositorioMock.Object, servComandosMock.Object, listaWorkflowsMock.Object, configuracion.Object);

            graficos = new List<GraficoDePlantaDto>
            {
                new GraficoDePlantaDto
                {
                    Id = 1,
                    NombreActividad = "Actividad 1",
                    CantidadCamionesNoDemorados = 18,
                    CantidadCamionesDemorados = 14,
                    Color = "Rojo",
                    Rango = 10
                },
                new GraficoDePlantaDto
                {
                    Id = 2,
                    NombreActividad = "Actividad 2",
                    CantidadCamionesNoDemorados = 14,
                    CantidadCamionesDemorados = 11,
                    Color = "Verde",
                    Rango = 14
                }
            };
            estados = new List<EstadoPlantaDto>
            {
                new EstadoPlantaDto
                {
                    TotalCamiones  = 200,
                    TotalCamionesDiarioSoja = 40,
                    TotalCamionesDiarioMaiz = 20,
                    TotalSojaIngresado = 40,
                    TotalMaizIngresado = 20,
                }
            };
        }

        [Test]
        public void TestModificarActividadExistente()
        {
            servRepositorioMock.Setup(s => s.ObtenerGraficoDePlanta("Actividad 1", 0)).Returns(graficos.First());

            var result = target.Modificar(new DatosUsuario(), "Actividad 1") as ViewResult;

            servRepositorioMock.Verify(p => p.ObtenerGraficoDePlanta("Actividad 1", 0), Times.Exactly(1));

            Assert.That(((GraficoDePlantaDto) result.Model).NombreActividad, Is.EqualTo("Actividad 1"));
            Assert.That(((GraficoDePlantaDto) result.Model).Id, Is.EqualTo(1));
            Assert.That(((GraficoDePlantaDto) result.Model).CantidadCamionesDemorados, Is.EqualTo(14));
            Assert.That(((GraficoDePlantaDto) result.Model).CantidadCamionesNoDemorados, Is.EqualTo(18));
            Assert.That(((GraficoDePlantaDto) result.Model).Color, Is.EqualTo("Rojo"));
            Assert.That(((GraficoDePlantaDto) result.Model).Rango, Is.EqualTo(10));

            Assert.That(result.ViewName, Is.EqualTo("Modificar"));
        }

        [Test]
        public void TestModificarActividadNoExistente()
        {
            servRepositorioMock.Setup(s => s.ObtenerGraficoDePlanta("Actividad 3", 0)).Returns((GraficoDePlantaDto) null);

            var result = target.Modificar(new DatosUsuario(), "Actividad 3") as ViewResult;

            servRepositorioMock.Verify(p => p.ObtenerGraficoDePlanta(It.IsAny<string>(), 0), Times.Exactly(1));

            Assert.That(target.ViewBag.NombreActividad, Is.EqualTo("Actividad 3"));
            Assert.That(target.ViewBag.CentroId, Is.EqualTo(0));
            Assert.IsNull((GraficoDePlantaDto) result.Model);
            Assert.That(result.ViewName, Is.EqualTo("Modificar"));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarGraficoDePlanta>()))
                .Returns(new Resultado());

            var result = target.Modificar(new DatosUsuario(),graficos.First()) as ContentResult;
            var expectedResult = new ContentResult {Content = "ajax-edit-success"};

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarGraficoDePlanta>()))
                .Returns(new Resultado());

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(new DatosUsuario(), graficos.First()) as ViewResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.IsNotNull(result.Model);
            Assert.That(target.ViewBag.CentroId, Is.EqualTo(0));
            Assert.That(result.ViewBag.NombreActividad, Is.EqualTo("Actividad 1"));
            Assert.AreNotEqual(result, expectedResult.Content);
            Assert.That(((GraficoDePlantaDto) result.Model).Id, Is.EqualTo(1));
            Assert.That(((GraficoDePlantaDto)result.Model).NombreActividad, Is.EqualTo("Actividad 1"));
            Assert.That(((GraficoDePlantaDto)result.Model).CantidadCamionesNoDemorados, Is.EqualTo(18));
            Assert.That(((GraficoDePlantaDto)result.Model).CantidadCamionesDemorados, Is.EqualTo(14));
            Assert.That(((GraficoDePlantaDto)result.Model).Color, Is.EqualTo("Rojo"));
            Assert.That(((GraficoDePlantaDto)result.Model).Rango, Is.EqualTo(10));
        }
    }
}