using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Molinos.Scato.Test.Controllers
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CargarCartaPorteByPassControllerTest
    {
        private const string WorkflowCodigo = "W1";
        private const int CentroId = 1;
        private const int CentroConfiguradoId = 42;
        private const int MaterialId = 7;
        private const int TipoComercialId = 8;
        private const int TransportistaId = 9;
        private const int TitularId = 10;
        private const int RtteComercialId = 11;
        private const string ChoferCuil = "20-12345678-3";
        private const string TransportistaCuit = "30-12345678-9";
        private const string ComplianceError = "Error de compliance";

        private CargarCartaPorteByPassController target;
        private Mock<IServicioRepositorio> servicioRepositorioMock;
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<IServicioActividadFactory<ICargarCartaPorteByPassService>> factoryMock;
        private Mock<IListaDeWorkflows> workflowsMock;
        private Mock<IFirmaProvider> configuracionMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<IServicioOrquestador> servicioOrquestadorMock;
        private CartaPorteDto orden;

        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";

            servicioRepositorioMock = new Mock<IServicioRepositorio>();
            servicioComandosMock = new Mock<IServicioComandos>();
            factoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteByPassService>>();
            workflowsMock = new Mock<IListaDeWorkflows>();
            configuracionMock = new Mock<IFirmaProvider>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            servicioOrquestadorMock = new Mock<IServicioOrquestador>();

            target = new CargarCartaPorteByPassController(
                new NullLogger(),
                servicioRepositorioMock.Object,
                factoryMock.Object,
                servicioComandosMock.Object,
                workflowsMock.Object,
                configuracionMock.Object,
                servicioSapMock.Object,
                servicioOrquestadorMock.Object);

            orden = CrearOrdenBase();
            ConfigurarEscenarioComun();
        }

        [Test]
        public void Index_CuandoConfiguracionCentroEsValida_UsaElCentroConfiguradoParaLaExcepcion()
        {
            ConfigurarCentroDestino(CentroConfiguradoId.ToString());

            var result = target.Index(WorkflowCodigo, string.Empty, string.Empty, string.Empty, orden, new DatosUsuario { CentroId = CentroId }) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.False);
            Assert.That(target.ModelState.Values.SelectMany(v => v.Errors).Single().ErrorMessage, Is.EqualTo(ComplianceError));

            servicioRepositorioMock.Verify(s => s.ObtenerConfiguracionGeneral(
                Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass,
                Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro,
                It.IsAny<int?>()), Times.Once());
            servicioRepositorioMock.Verify(s => s.ExisteExcepcionAlControlParaCartaPorte(
                orden.MaterialId,
                orden.TransportistaId ?? 0,
                CentroId,
                It.IsAny<DateTime>(),
                CentroConfiguradoId,
                null), Times.Once());
            servicioComandosMock.Verify(s => s.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            servicioComandosMock.Verify(s => s.Ejecutar(It.IsAny<VerificarEnCompliance>()), Times.Once());
        }

        [Test]
        public void Index_CuandoConfiguracionCentroNoEsNumerica_NoConsultaLaExcepcionDeCartaPorte()
        {
            ConfigurarCentroDestino("ABC");

            var result = target.Index(WorkflowCodigo, string.Empty, string.Empty, string.Empty, orden, new DatosUsuario { CentroId = CentroId }) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.False);
            Assert.That(target.ModelState.Values.SelectMany(v => v.Errors).Single().ErrorMessage, Is.EqualTo(ComplianceError));

            servicioRepositorioMock.Verify(s => s.ObtenerConfiguracionGeneral(
                Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass,
                Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro,
                It.IsAny<int?>()), Times.Once());
            servicioRepositorioMock.Verify(s => s.ExisteExcepcionAlControlParaCartaPorte(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()), Times.Never());
            servicioComandosMock.Verify(s => s.Ejecutar(It.IsAny<VerificarEnCompliance>()), Times.Once());
        }

        private void ConfigurarEscenarioComun()
        {
            configuracionMock.Setup(s => s.ObtenerFirmaSinLogo())
                .Returns(new FirmaDto { CodigoSAP = "9999999999" });

            servicioRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>()))
                .Returns(new WorkflowDto
                {
                    Id = 1,
                    Codigo = WorkflowCodigo,
                    Descripcion = "Cargar Carta Porte ByPass",
                    TipoDeWorkflow = TipoDeWorkflow.Ingreso,
                    Activo = true
                });

            servicioRepositorioMock.Setup(s => s.NumeroCartaPorteValido(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new CartaPorteValidaResponseDto { Valida = true });

            servicioRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                .Returns(new TipoComercialDto
                {
                    Id = TipoComercialId,
                    Descripcion = "Tipo Comercial",
                    TransportistaEsProveedor = false
                });

            servicioRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(f => f.Cuil == ChoferCuil)))
                .Returns(new List<ChoferDto>
                {
                    new ChoferDto
                    {
                        Id = 77,
                        Cuil = ChoferCuil,
                        Nombre = "Juan",
                        Apellido = "Perez",
                        NumeroDeDocumento = "12345678",
                        TipoDocumentoIdentidadId = 1
                    }
                });

            servicioComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>()))
                .Returns(new Resultado());

            servicioRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                .Returns<int>(id =>
                {
                    if (id == TitularId)
                    {
                        return new ProveedorDto
                        {
                            Id = id,
                            CodigoSap = "103",
                            Cuil = "20-00000010-1",
                            RazonSocial = "Titular"
                        };
                    }

                    if (id == RtteComercialId)
                    {
                        return new ProveedorDto
                        {
                            Id = id,
                            CodigoSap = "104",
                            Cuil = "20-00000011-1",
                            RazonSocial = "Remitente"
                        };
                    }

                    return new ProveedorDto
                    {
                        Id = id,
                        CodigoSap = "105",
                        Cuil = "20-00000012-2",
                        RazonSocial = "Otro"
                    };
                });

            servicioRepositorioMock.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                .Returns(new TransportistaDto
                {
                    Id = TransportistaId,
                    Cuit = TransportistaCuit,
                    RazonSocial = "Transportes SA"
                });

            servicioRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
                .Returns((OtroRecorridoDelChoferDto)null);

            servicioRepositorioMock.Setup(s => s.ObtenerCodigoEstablecimientoEsDeMolinos(It.IsAny<string>()))
                .Returns(false);

            servicioRepositorioMock.Setup(s => s.TomaFotoEnMesa(It.IsAny<int>()))
                .Returns(false);

            servicioRepositorioMock.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<MaterialPorWorkflowDto>());

            servicioRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>()))
                .Returns(new List<TipoComercialDto>());

            servicioRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(new CentroDto { Id = CentroId });

            servicioRepositorioMock.Setup(s => s.ListarRamalFerroviario())
                .Returns(new List<RamalFerroviarioDto>());

            servicioRepositorioMock.Setup(s => s.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>()))
                .Returns(new List<PesoMaximoPorTipoVehiculoDto>());

            servicioRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(new List<TipoDocumentoIdentidadDto>());

            servicioRepositorioMock.Setup(s => s.ListarCategorias())
                .Returns(new List<CategoriaDto>());

            servicioComandosMock.Setup(s => s.Ejecutar(It.IsAny<VerificarEnCompliance>()))
                .Returns(() =>
                {
                    var resultado = new ResultadoValidarCompliance();
                    resultado.Error(string.Empty, ComplianceError);
                    return resultado;
                });

            servicioRepositorioMock.Setup(s => s.ExisteExcepcionAlControlParaCartaPorte(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()))
                .Returns(false);
        }

        private void ConfigurarCentroDestino(string valorCentroDestino)
        {
            servicioRepositorioMock.Setup(s => s.ObtenerConfiguracionGeneral(
                    Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass,
                    Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro,
                    It.IsAny<int?>()))
                .Returns(new ConfiguracionGeneralDto { Valor = valorCentroDestino });
        }

        private static CartaPorteDto CrearOrdenBase()
        {
            return new CartaPorteDto
            {
                NroCartaPorte = "12345678",
                CTG = "12345678",
                FechaEmision = DateTime.Today,
                FechaCP = DateTime.Today,
                FechaVto = DateTime.Today.AddDays(1),
                TipoVehiculo = TipoVehiculo.Camión,
                OrigenVehiculo = OrigenVehiculo.Argentina,
                TipoComercialId = TipoComercialId,
                TitularCartaPorteId = TitularId,
                RtteComercialId = RtteComercialId,
                TransportistaId = TransportistaId,
                MaterialId = MaterialId,
                CodEstab = "123456",
                Cpe = false,
                EsTransportista = true,
                RequiereCupo = false,
                ValidarCupo = false,
                Cupo = "MOL0001/00000001",
                DestinoId = 99,
                Destino = "Destino",
                IntermediarioFleteId = 0,
                Chofer = new ChoferDto
                {
                    Id = 1,
                    Cuil = ChoferCuil,
                    Nombre = "Juan",
                    Apellido = "Perez",
                    NumeroDeDocumento = "12345678",
                    TipoDocumentoIdentidadId = 1
                },
                Vehiculos = new List<VehiculoDto>
                {
                    new VehiculoDto
                    {
                        Patente = "AAA111"
                    }
                }
            };
        }
    }
}
