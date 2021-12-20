using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
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
    public class CargarCartaPorteRedespachoDesvioControllerTest
    {
        private CargarCartaPorteRedespachoDesvioController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<ICargarCartaPorteRedespachoDesvioService>> actFactoryMock;
        private Mock<IFirmaProvider> configuracion;
        private Mock<ICargarCartaPorteRedespachoDesvioService> contractMock;


        private CartaPorteDto dto;
        private VehiculoDto vehiculo;
        private List<TipoComercialDto> tiposComerciales;
        private List<MaterialPorWorkflowDto> materiales;
        private List<TipoDocumentoIdentidadDto> tipos;
        private List<BocaDestinoDto> bocaDestino;
        private List<WorkflowDto> workflows;

        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;

        private NullLogger logger;

        private Guid InstanciaWfId = Guid.NewGuid();
        private const string workflow = "W1";

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteRedespachoDesvioService>>();
            configuracion = new Mock<IFirmaProvider>();
            logger = new NullLogger();
            target = new CargarCartaPorteRedespachoDesvioController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, configuracion.Object);
            contractMock = new Mock<ICargarCartaPorteRedespachoDesvioService>();
            vehiculo = new VehiculoDto();
            dto = new CartaPorteDto()
            {
                TipoVehiculo = TipoVehiculo.Camión,
                NroCartaPorte = "77777777",
                CTG = "77777777",
                FechaCP = DateTime.Now.AddDays(2),
                TipoComercial = "TipoComercialDesc",
                CEE = "77777777",
                FechaEmision = DateTime.Now,
                FechaVto = DateTime.Now.AddDays(10),
                TitularCartaPorte = "Test",
                Destinatario = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne",  NumeroDeDocumento = "123" },
                Cosecha = "12-13",
                Procedencia = "Laurencena",
                OrigenVehiculo = OrigenVehiculo.Argentina,
                KmRecorrer = 798,
                TarifaTonelada = 12,
                FleteAPagar = true,
                Destino = "4",
                Vehiculos = new List<VehiculoDto>{vehiculo},
                EsTransportista = true
            };
            vehiculo = new VehiculoDto
            {
                Patente = "AAA111"
            };
            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 } };
            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            bocaDestino = new List<BocaDestinoDto> { new BocaDestinoDto() { Id = 1, NombreBocaDeDestino = "B1" } };
            workflows = new List<WorkflowDto> { new WorkflowDto() { Id = 1, Codigo = "W1", Descripcion = "W1" } };

            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorioMock.Setup(s => s.ListarBocasDestino()).Returns(bocaDestino);

            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto{Workflow = workflows[0], Centro = new CentroDto()});
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1", Activo = true });
            servRepositorioMock.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            servRepositorioMock.Setup(s => s.NumeroCartaPorteValidoRedespacho(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new CartaPorteValidaResponseDto { Valida = true });
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial" });
            contractMock.Setup(s => s.CargarCartaPorteRedespachoDesvio(It.IsAny<Guid>(), It.IsAny<CartaPorteDto>(),It.IsAny<VehiculoDto>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = InstanciaWfId });
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            configuracion.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "9950085862" });
            servRepositorioMock.Setup(s => s.ObtenerProveedorPorCodigoSap(It.IsAny<string>())).Returns(new ProveedorDto { Id = 1, Descripcion = "Proveedor 1" });
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            servRepositorioMock.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns(new CartaPorteDto());
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());

            pesosMax = new List<PesoMaximoPorTipoVehiculoDto>()
            {
                new PesoMaximoPorTipoVehiculoDto()
                {
                    Activo = true,
                    CentroId = 1,
                    Id = 1,
                    PesoMaxEgreso = 65000,
                    PesoMaxIngreso = 65000,
                    TipoVehiculo = TipoVehiculo.Bitren
                }
            };

            servRepositorioMock.Setup(x => x.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>())).Returns(pesosMax);
        }

        [Test]
        public void TestIndexSinCp()
        {
            servRepositorioMock.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns((CartaPorteDto) null);
            var result = target.Index(It.IsAny<Guid>(), new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.EgresoPorDesvioError));      
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario() { CentroId = 1 };
            var result = target.Index(It.IsAny<Guid>(), datosUsuario) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }

        [Test]
        public void TestIndexPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index(dto, datosUsuario) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            contractMock.Verify(v => v.CargarCartaPorteRedespachoDesvio(It.IsAny<Guid>(), It.IsAny<CartaPorteDto>(), It.IsAny<VehiculoDto>(), It.IsAny<ControlRecorridoDto>()), Times.Once());
        }

        [Test]
        public void TestNroCartaPorteInvalida()
        {
            servRepositorioMock.Setup(s => s.NumeroCartaPorteValidoRedespacho(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new CartaPorteValidaResponseDto { Valida = false, CodigoDeError = 1, Error = "Error Test" });

            var result = target.Index(dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Error Test"));
        }

        [Test]
        public void TestChoferEnOtroRecorrido()
        {
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
               .Returns(new OtroRecorridoDelChoferDto { NumeroDocumentoIngreso = "77777777", Patente = "AAA111" });

            var result = target.Index(dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_ChoferYaEstaEnPlanta, dto.Chofer.NombreCompleto, dto.NroCartaPorte, vehiculo.Patente)));
        }
    }
}
