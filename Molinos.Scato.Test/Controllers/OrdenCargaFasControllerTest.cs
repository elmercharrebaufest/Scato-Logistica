using System;
using System.Collections.Generic;
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
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class IngresarOrdenCargaFasControllerTest
    {
        private IngresarOrdenCargaFasController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>> actFactoryMock;
        private Mock<IIngresarOrdenCargaFasService> contractMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;

        private Mock<IListaDeWorkflows> listaMock;

        private OrdenCargaFasDto dto;
        private List<TipoComercialDto> tiposComerciales;
        private List<MaterialDto> materiales;
        private List<TipoDocumentoIdentidadDto> tipos;
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;
        private DatosUsuario dtoDatosUsuario;

        private NullLogger logger;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>>();
            contractMock = new Mock<IIngresarOrdenCargaFasService>();
            listaMock = new Mock<IListaDeWorkflows>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            logger = new NullLogger();
            target = new IngresarOrdenCargaFasController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, listaMock.Object, servicioSapMock.Object);
            dtoDatosUsuario = new DatosUsuario { 
                BalanzaId = 1,  
                CentroId = 5
            };

            dto = new OrdenCargaFasDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Roberto", Apellido = "Sanchez", NumeroDeDocumento = "123"},
                ClienteId = 1,
                NumeroOrden = "1",
                PatenteAcoplado = "AAA222",
                MaterialId = 1,
                PatenteCamion = "AAA111",
                TipoComercialId = 1,
                TransportistaId = 1,
                MaterialDesc = "1",
                ClienteDesc = "1",
                TipoComercialDesc = "1",
                ValidaCompliance = false,
                TransportistaDesc = "1",
                CuitTransporte = "1",
                Id = 1
            };

            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

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

            materiales = new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "M1" } };
            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto {Id = 1, DescripcionCorta = "T1"} };

            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorioMock.Setup(s => s.ObtenerProveedorPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(new ProveedorDto()
                {
                    Id = 1,
                    RazonSocial = "1",
                    Cuil = "2",
                    CodigoSap = "3"
                });
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto {Cuil = "123"});
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>())).Returns(new List<ChoferDto> { new ChoferDto()});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(new ResultadoCrear { Id = 1 });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
        }

        [Test]
        public void TestChoferNoSeleccionadoCuilExistenteMismoChofer()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });
            contractMock.Setup(s => s.IngresarOrdenCargaFas(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), false, It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = new Guid() });

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }, string.Empty, 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestChoferNoSeleccionadoCuilNoExistente()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            contractMock.Setup(s => s.IngresarOrdenCargaFas(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), false, It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = new Guid() });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(new ResultadoCrear { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }, string.Empty, 1) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestChoferSeleccionado()
        {
            const string workflow = "W1";
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.IngresarOrdenCargaFas(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), false, It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = new Guid() });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(new ResultadoCrear { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }, string.Empty, 1) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestCentroNoSeleccionado()
        {
            const string workflow = "W1";
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });
            contractMock.Setup(s => s.IngresarOrdenCargaFas(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), false, It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = new Guid() });

            var result = target.Index(workflow, dto, new DatosUsuario(), string.Empty, 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestPatenteEnOtroWorkflow()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });
            contractMock.Setup(s => s.IngresarOrdenCargaFas(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), false, It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = new Guid() });
            listaMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto());

            var result = target.Index(workflow, dtoDatosUsuario, new DatosUsuario{CentroId = 1}.BalanzaId ) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            //Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            //Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.OrdenCargaInterna_PatenteEnOtroWorkflow));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }
    }
}
