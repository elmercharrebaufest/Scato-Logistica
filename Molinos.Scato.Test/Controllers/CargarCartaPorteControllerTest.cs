using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
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
    public class CargarCartaPorteControllerTest
    {
        private CargarCartaPorteController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<ICargarCartaPorteService>> actFactoryMock;
        private Mock<ICargarCartaPorteService> contractMock;

        private Mock<IListaDeWorkflows> listaMock;

        private CartaPorteDto dto;
        private VehiculoDto vehiculo;
        private List<TipoComercialDto> tiposComerciales;
        private List<MaterialPorWorkflowDto> materiales;
        private List<TipoDocumentoIdentidadDto> tipos;
        private List<BocaDestinoDto> bocaDestino;
        private List<WorkflowDto> workflows;
        private Mock<IFirmaProvider> configuracion;
        private NullLogger logger;
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;
        private Mock<ZSDWS_SCATO> servicioSap;

        private Guid InstanciaWfId = Guid.NewGuid();
        private const string workflow = "W1";

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteService>>();
            contractMock = new Mock<ICargarCartaPorteService>();
            listaMock = new Mock<IListaDeWorkflows>();
            configuracion = new Mock<IFirmaProvider>();
            logger = new NullLogger();
            servicioSap = new Mock<ZSDWS_SCATO>();
            target = new CargarCartaPorteController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, listaMock.Object, configuracion.Object, servicioSap.Object, null);

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
                TransportistaId = 1,
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123"},
                Cosecha = "12-13",
                Procedencia = "Laurencena",
                OrigenVehiculo = OrigenVehiculo.Argentina,
                KmRecorrer = 798,
                TarifaTonelada = 12,
                FleteAPagar = true,
                Destino = "4",
                VehiculoJson = "[{\"Patente\":\"aaa111\",\"PatenteAcoplado\":null,\"PesoBrutoOrigen\":\"40000\",\"PesoTaraOrigen\":\"1000\",\"PesoNetoOrigen\":39000,\"Primero\":true}]",
                EsTransportista = true,
                TitularCartaPorteId = 1,
                RtteComercialId = 2
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
            materiales = new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1}};
            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto {Id = 1, DescripcionCorta = "T1"} };
            bocaDestino = new List<BocaDestinoDto> { new BocaDestinoDto() { Id = 1, NombreBocaDeDestino = "B1" } };
            workflows = new List<WorkflowDto>{new WorkflowDto(){Id = 1, Codigo = "W1", Descripcion = "W1"}};
            configuracion.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { CodigoSAP = "9950085862" });
            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorioMock.Setup(s => s.ListarBocasDestino()).Returns(bocaDestino);
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1", Activo = true});
            servRepositorioMock.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            servRepositorioMock.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(true);
            servRepositorioMock.Setup(s => s.NumeroCartaPorteValido(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(new CartaPorteValidaResponseDto{Valida = true});
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial" });
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.CargarCartaPorte(It.IsAny<CartaPorteDto>(), It.IsAny<VehiculoDto>(), It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = InstanciaWfId });
            servRepositorioMock.Setup(s => s.ObtenerCartaPorteVacia(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new CartaPorteDto());
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, Descripcion = "Proveedor 1", RequiereCupo = false });

            servRepositorioMock.Setup(x => x.ObtenerProveedor(1)).Returns(new ProveedorDto { CodigoSap = "9950085862" });
            servRepositorioMock.Setup(x => x.ObtenerProveedor(2)).Returns(new ProveedorDto { CodigoSap = "9950085863" });

            servRepositorioMock.Setup(s => s.ListarCategorias())
                .Returns(new List<CategoriaDto> { new CategoriaDto { Id = 1, Clasificacion = "clasificacion1" }, new CategoriaDto { Id = 2, Clasificacion = "clasificacion2" } });
        }

        [Test]
        public void TestIndex()
        {
            DatosUsuario datosUsuario = new DatosUsuario() { CentroId = 1 };

            var result = target.Index("W1", datosUsuario) as ViewResult;
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
        public void TestChoferActualizacion()
        {
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestChoferActualizacionError()
        {
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);
            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }

        [Test]
        public void TestChoferNuevo()
        {
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto>());
            
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Once());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestChoferNuevoError()
        {
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto> { });
            
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(resultado);
            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportista()
        {
            dto.EsTransportista = false;
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";

            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(new ResultadoCrear());
            
            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Once());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestTransportistaEsProveedorTransportistaExistente()
        {
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servRepositorioMock.Setup(s => s.ObtenerTransportistaPorCuit(It.IsAny<string>())).Returns(new TransportistaDto());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });

            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Never());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportistaError()
        {
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";
            dto.EsTransportista = false;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                .Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto>());
            
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(resultado);

            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Once());
        }

        [Test]
        public void TestSinTransportistaObligatorio()
        {
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";
            dto.TransportistaId = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                .Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>()))
                .Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                .Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
           
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>()))
                .Returns(resultado);

            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_Requerido, Textos.Transportista)));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Never());
        }


        [Test]
        public void TestChoferEnOtroRecorrido()
        {
            ConfigurationManager.AppSettings["CodigoSapMRP"] = "50085862";
            dto.TransportistaId = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                .Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>()))
                .Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                .Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
                .Returns(new OtroRecorridoDelChoferDto { NumeroDocumentoIngreso = "77777777", Patente = "OtroRe" });

            var result = target.Index(workflow, "", "", "", dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_ChoferYaEstaEnPlanta, dto.Chofer.NombreCompleto, dto.NroCartaPorte, "OtroRe")));

        }

        private void ValidarVistaConError(ViewResult result)
        {
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ViewBag.EsIngreso, Is.EqualTo(true));
        }
    }
}
