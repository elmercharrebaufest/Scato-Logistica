using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    internal class IngresoDeDatosDeExportacionControllerTest
    {
        private IngresoDeDatosDeExportacionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IConfiguracionProvider> configuracionMock;
        private Mock<IFirmaProvider> firmaMock;
        private ILogger logger;
        private DatosUsuario datos;
        private RecorridoDto recorrido;
        private Guid id;
        private ControlRecorridoDto controlRecorrido;
        private Mock<IIngresoDeDatosDeExportacionService> contract;
        private IngresoDeDatosDeExportacionDto dto;
        private Mock<IServicioActividadFactory<IIngresoDeDatosDeExportacionService>> factoryMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            configuracionMock = new Mock<IConfiguracionProvider>();
            contract = new Mock<IIngresoDeDatosDeExportacionService>();
            firmaMock = new Mock<IFirmaProvider>();
            factoryMock = new Mock<IServicioActividadFactory<IIngresoDeDatosDeExportacionService>>();
            logger = new NullLogger();
            
            datos = new DatosUsuario { CentroDescripcion = "centro 1", PuestoDeTrabajoId = 1, NombreUsuario = "Usuario 1"};
            id = new Guid("2E024D75-7E1B-4C6E-82A1-41C3B0FDEAB3");
            var firmas = new List<FirmaDto>
            {
                new FirmaDto {Descripcion = "Firma1"},
                new FirmaDto {Descripcion = "Firma2"}
            };

            var descCentro = datos.CentroDescripcion;
            var material = new MaterialDto { Descripcion = "Soja" };

            recorrido = new RecorridoDto
            {
                InstanciaWorkflow = id,
                Centro = new CentroDto { Descripcion = descCentro },
                TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                NumeroDocumentoIngreso = "6540654060",
                Patente = "AAA111",
                Material = material,
                WorkflowDefinicionId = 1
            };

            controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresoDeDatosDeExportacion,
                ActividadXaml = "IngresoDeDatosDeExportacion",
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                PuestoDeTrabajoId = datos.PuestoDeTrabajoId,
                NombreUsuario = datos.NombreUsuario
            };

            dto = new IngresoDeDatosDeExportacionDto { InstanciaWorkflow = id, WorkflowDefinicionId = recorrido.WorkflowDefinicionId };

            servRepositorioMock.Setup(s => s.ListarPaises()).Returns(new List<PaisDto> { new PaisDto { Descripcion = "Argentina", Id = 2 }, new PaisDto { Descripcion = "Uruguay", Id = 3 } });
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(id)).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarFirmas()).Returns(firmas);
            factoryMock.Setup(x => x.CrearServicio(recorrido.WorkflowDefinicionId)).Returns(contract.Object);
            contract.Setup(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>())).Returns(new Resultado());

            target = new IngresoDeDatosDeExportacionController(logger, factoryMock.Object, servRepositorioMock.Object);
        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index(id) as ViewResult;
            var model = (IngresoDeDatosDeExportacionDto)result.Model;

            Assert.That((string)result.ViewBag.TipoDocumentoIngreso, Is.EqualTo(TipoDocumentoIngreso.CartaPorte.ToString()));
            Assert.That((string)result.ViewBag.NumeroDocumentoIngreso, Is.EqualTo(recorrido.NumeroDocumentoIngreso));
            Assert.That((string)result.ViewBag.Patente, Is.EqualTo(recorrido.Patente));
            Assert.That((string)result.ViewBag.Material, Is.EqualTo(recorrido.Material.Descripcion));

            Assert.That(result.ViewBag.Firmas, Is.Not.Null);
            Assert.That(result.ViewBag.Nacionalidades, Is.Not.Null);

            Assert.That(model.WorkflowDefinicionId, Is.EqualTo(1));
            Assert.That(model.InstanciaWorkflow, Is.EqualTo(id));
            Assert.That(result.ViewName, Is.Null.Or.Empty);

            servRepositorioMock.Verify(x => x.ObtenerRecorridoPorGuid(id), Times.Exactly(1));
            servRepositorioMock.Verify(x => x.ListarFirmas(), Times.Exactly(1));
            servRepositorioMock.Verify(x => x.ListarPaises(), Times.Exactly(1));
        }

        [Test]
        public void TestIndexPost()
        {
            var resultado = target.Index(dto, datos) as RedirectToRouteResult;

            Assert.That(resultado, Is.Not.Null);
            factoryMock.Verify(x => x.CrearServicio(recorrido.WorkflowDefinicionId), Times.Exactly(1));
            contract.Verify(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>()), Times.Exactly(1));

            Assert.That(resultado.RouteValues.First(f => f.Key == "action").Value, Is.EqualTo("Index"));
            Assert.That(resultado.RouteValues.First(f => f.Key == "controller").Value, Is.EqualTo("ListaDeCamiones"));
        }

        [Test]
        public void TestIndexPostConError()
        {
            ResultadoCrear resultadoConError = new ResultadoCrear();
            resultadoConError.Errores.Add("Error", "Error");
            contract.Setup(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>())).Returns(resultadoConError);

            var resultado = target.Index(dto, datos) as RedirectToRouteResult;

            Assert.That(resultado, Is.Not.Null);
            factoryMock.Verify(x => x.CrearServicio(recorrido.WorkflowDefinicionId), Times.Exactly(1));
            contract.Verify(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>()), Times.Exactly(1));

            Assert.That(resultado.RouteValues.First(f => f.Key == "action").Value, Is.EqualTo("Index"));
            Assert.That(resultado.RouteValues.First(f => f.Key == "id").Value, Is.EqualTo(id));
        }

        [Test]
        public void TestIndexPostInvalido()
        {
            contract.Setup(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>())).Returns(new Resultado());

            target.ModelState.AddModelError("", "Error");
            var resultado = target.Index(dto, datos) as ViewResult;

            Assert.That(resultado, Is.Not.Null);
            factoryMock.Verify(x => x.CrearServicio(recorrido.WorkflowDefinicionId), Times.Exactly(0));
            contract.Verify(x => x.IngresoDeDatosDeExportacion(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<IngresoDeDatosDeExportacionDto>()), Times.Exactly(0));

            Assert.That((string)resultado.ViewBag.TipoDocumentoIngreso, Is.EqualTo(TipoDocumentoIngreso.CartaPorte.ToString()));
            Assert.That((string)resultado.ViewBag.NumeroDocumentoIngreso, Is.EqualTo(recorrido.NumeroDocumentoIngreso));
            Assert.That((string)resultado.ViewBag.Patente, Is.EqualTo(recorrido.Patente));
            Assert.That((string)resultado.ViewBag.Material, Is.EqualTo(recorrido.Material.Descripcion));

            Assert.That(resultado.ViewBag.Firmas, Is.Not.Null);
            Assert.That(resultado.ViewBag.Nacionalidades, Is.Not.Null);

            servRepositorioMock.Verify(x => x.ObtenerRecorridoPorGuid(id), Times.Exactly(1));
            Assert.That(resultado.ViewName, Is.EqualTo(String.Empty));
        }
    }
}