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
    public class DescargaUnidadControllerTest
    {
        private DescargaUnidadController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IDescargaUnidadService>> actFactoryMock;
        private Mock<IDescargaUnidadService> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;

        private OrdenDeDescargaDto OrdenDeDescargaDto;
        private RecorridoDto recorridoDto;
        private DescargaUnidadDto descargaUnidadDto;
        private List<MaterialDto> materiales;
        private List<AlmacenDto> almacenes;

        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IDescargaUnidadService>>();
            contractMock = new Mock<IDescargaUnidadService>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            logger = new NullLogger();
            target = new DescargaUnidadController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, orquestadorMock.Object);

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

            descargaUnidadDto = new DescargaUnidadDto()
                {
                    NroDescarga = 1,
                    DescargaUnidadItems = new List<DescargaUnidadItemDto>(),
                    DescargaUnidadItemPedidos = new List<DescargaUnidadItemPedidoDto>() { new DescargaUnidadItemPedidoDto() { MaterialId = 1, MaterialDescripcion = "test" } }
                };

            materiales = new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "Mat1"} };
            almacenes = new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "Alm1", DescripcionCorta = "Alm1" } };
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1" });
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>())).Returns(OrdenDeDescargaDto);
            servRepositorioMock.Setup(s => s.ObtenerUltimaDescargaUnidadPorGuid(It.IsAny<Guid>())).Returns(descargaUnidadDto);
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidad(It.IsAny<int>())).Returns(descargaUnidadDto);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaPorInstanceId(It.IsAny<Guid>())).Returns(OrdenDeDescargaDto);
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidadPorGuid(It.IsAny<Guid>())).Returns(new List<DescargaUnidadDto> { descargaUnidadDto });
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidadProveedor(It.IsAny<int>())).Returns(new DescargaUnidadDto() { DescargaUnidadItems = new List<DescargaUnidadItemDto>() });
            servRepositorioMock.Setup(s => s.ListarTaraRomaneosPorCentro(It.IsAny<int>()))
                               .Returns(new List<TaraRomaneoDto>(){ new TaraRomaneoDto()});
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns( new List<BalanzaDto>());
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
        public void TestTerminarDescargaUnidadAceptar()
        {
            descargaUnidadDto.Estado = EstadoDescargaUnidad.EnProceso;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarDescargaUnidad>())).Returns(new Resultado());
            target.TerminarDescarga(descargaUnidadDto);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ActualizarDescargaUnidad>()), Times.Once());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.DescargaUnidades_Guardado));
        }
        [Test]
        public void TestIndexPostAceptarError()
        {
            descargaUnidadDto.Estado = EstadoDescargaUnidad.EnProceso;
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ActualizarDescargaUnidad>())).Returns(resultado);
            var result = target.TerminarDescarga(descargaUnidadDto) as ViewResult;
            Assert.That((string)target.ViewBag.FechaIngreso, Is.EqualTo(recorridoDto.FechaInicio.Formatted()));
            Assert.That((string)target.ViewBag.Patente, Is.EqualTo(OrdenDeDescargaDto.PatenteCamion));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ActualizarDescargaUnidad>()), Times.Once());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Error));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.DescargaUnidades_Error));
        }
        
        [Test]
        public void TestIndexPostFinalizarError()
        {
            descargaUnidadDto.Estado = EstadoDescargaUnidad.Finalizado;
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            contractMock.Setup(s => s.DescargaUnidad(It.IsAny<Guid>(), false, It.IsAny<ControlRecorridoDto>())).Returns(resultado);
            var result = target.TerminarVehiculo(descargaUnidadDto, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "usuario"}) as ViewResult;
            Assert.That((string)target.ViewBag.FechaIngreso, Is.EqualTo(recorridoDto.FechaInicio.Formatted()));
            Assert.That((string)target.ViewBag.Patente, Is.EqualTo(OrdenDeDescargaDto.PatenteCamion));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Error));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.DescargaUnidad_Error));
        }
        
        [Test]
        public void TestDescargarItemPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearDescargaUnidadItem>())).Returns(new Resultado());
            var result = target.DescargarItem(It.IsAny<DescargaUnidadItemDto>(), It.IsAny<bool>(), It.IsAny<int>()) as ContentResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearDescargaUnidadItem>()), Times.Once());
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

    }
}
