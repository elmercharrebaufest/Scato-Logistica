using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class RomaneoControllerTest
    {
        private RomaneoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IRomaneoService>> actFactoryMock;
        private Mock<IRomaneoService> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;

        private OrdenDeDescargaDto OrdenDeDescargaDto;
        private RecorridoDto recorridoDto;
        private RomaneoDto romaneoDto;
        private List<MaterialDto> materiales;
        private List<AlmacenDto> almacenes;

        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IRomaneoService>>();
            contractMock = new Mock<IRomaneoService>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            logger = new NullLogger();
            target = new RomaneoController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object);

            OrdenDeDescargaDto = new OrdenDeDescargaDto()
            {
                Numero = "77777777",
                FechaMovimiento = DateTime.Now.AddDays(2),
                TipoComercial = "TipoComercialDesc",
                Proveedor = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
                PatenteCamion = "AAA111"

            };

            recorridoDto = new RecorridoDto
                {
                    FechaInicio = new DateTime(2011, 1, 1),
                    Workflow = new WorkflowDto(),
                    Patente = "AAA111",
                    NumeroDocumentoIngreso = "77777777",
                };

            romaneoDto = new RomaneoDto
                {
                    Numero = 1,
                    RomaneoItems = new List<RomaneoItemDto>(),
                    RomaneoItemsPedidos = new List<RomaneoItemPedidoDto>(){ new RomaneoItemPedidoDto(){MaterialId = 1, MaterialDescripcion = "test"}}
                };

            materiales = new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "Mat1"} };
            almacenes = new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "Alm1", DescripcionCorta = "Alm1" } };
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1" });
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>())).Returns(OrdenDeDescargaDto);
            servRepositorioMock.Setup(s => s.ObtenerUltimoRomaneoPorGuid(It.IsAny<Guid>())).Returns(romaneoDto);
            servRepositorioMock.Setup(s => s.ObtenerRomaneo(It.IsAny<int>())).Returns(romaneoDto);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>())).Returns(OrdenDeDescargaDto);
            servRepositorioMock.Setup(s => s.ObtenerRomaneosPorGuid(It.IsAny<Guid>())).Returns(new List<RomaneoDto> { romaneoDto });
            servRepositorioMock.Setup(s => s.ObtenerRomaneoProveedor(It.IsAny<int>())).Returns(new RomaneoDto(){ RomaneoItems = new List<RomaneoItemDto>()});
            servRepositorioMock.Setup(s => s.ListarTaraRomaneosPorCentro(It.IsAny<int>()))
                               .Returns(new List<TaraRomaneoDto>(){ new TaraRomaneoDto()});
            servRepositorioMock.Setup(s => s.ListarBalanzasActivas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns( new List<BalanzaDto>());
        }
        [Test]
        public void TestIndex()
        {
            var result = target.Index(It.IsAny<Guid>()) as ViewResult; 
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That((string)target.ViewBag.FechaIngreso, Is.EqualTo(recorridoDto.FechaInicio.Formatted()));
            Assert.That((string)target.ViewBag.Patente, Is.EqualTo(OrdenDeDescargaDto.PatenteCamion));
        }
        [Test]
        public void TestTerminarRomaneoAceptar()
        {
            romaneoDto.Estado = EstadoRomaneo.EnProceso;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarRomaneo>())).Returns(new Resultado());
            target.TerminarRomaneo(romaneoDto);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ActualizarRomaneo>()), Times.Once());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Romaneo_Guardado));
        }
        [Test]
        public void TestIndexPostAceptarError()
        {
            romaneoDto.Estado = EstadoRomaneo.EnProceso;
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarRomaneo>())).Returns(resultado);
            var result = target.TerminarRomaneo(romaneoDto) as ViewResult;
            Assert.That((string)target.ViewBag.FechaIngreso, Is.EqualTo(recorridoDto.FechaInicio.Formatted()));
            Assert.That((string)target.ViewBag.Patente, Is.EqualTo(OrdenDeDescargaDto.PatenteCamion));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ActualizarRomaneo>()), Times.Once());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Error));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Romaneo_Error));
        }
        
        [Test]
        public void TestIndexPostFinalizarError()
        {
            romaneoDto.Estado = EstadoRomaneo.Finalizado;
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            contractMock.Setup(s => s.Romaneo(It.IsAny<Guid>(), false, It.IsAny<ControlRecorridoDto>())).Returns(resultado);
            var result = target.TerminarVehiculo(romaneoDto, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "Usuario"}) as ViewResult;
            Assert.That((string)target.ViewBag.FechaIngreso, Is.EqualTo(recorridoDto.FechaInicio.Formatted()));
            Assert.That((string)target.ViewBag.Patente, Is.EqualTo(OrdenDeDescargaDto.PatenteCamion));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Error));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Romaneo_Error));
        }
        
        [Test]
        public void TestDescargarItemPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRomaneoItem>())).Returns(new Resultado());
            var result = target.DescargarItem(It.IsAny<RomaneoItemDto>(), It.IsAny<bool>(), It.IsAny<int>()) as ContentResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearRomaneoItem>()), Times.Once());
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }
        [Test]
        public void TestDescargarItemPostProximoItem()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearRomaneoItem>())).Returns(new Resultado());
            var result = target.DescargarItem(new RomaneoItemDto(), true,1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("DescargarItem", result.RouteValues["action"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearRomaneoItem>()), Times.Once());
        }
    }
}
