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
    public class RemitoBodegaVinoControllerTest
    {
        private RemitoBodegaVinoController target;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;
        private Mock<IServicioActividadFactory<IRemitoBodegaVinoService>> actividadFactoryMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<IRemitoBodegaVinoService> contractMock;

        private Mock<IListaDeWorkflows> listaWorkflowsMock;

        private RemitoBodegaVinoDto dto;
        private List<TipoComercialDto> tiposComerciales;
        private List<TipoDocumentoIdentidadDto> tipoDeDocumentoDeIdentidad;
        private readonly Guid instanciaWorkflowId = Guid.NewGuid();
        private CentroDto centroDto;
        private NullLogger logger;
        private const string workflow = "WVino1";

        [SetUp]
        public void SetUp()
        {
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            actividadFactoryMock = new Mock<IServicioActividadFactory<IRemitoBodegaVinoService>>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            contractMock = new Mock<IRemitoBodegaVinoService>();
            listaWorkflowsMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();

            target = new RemitoBodegaVinoController(logger, srvRepositorio.Object,actividadFactoryMock.Object,srvComando.Object,listaWorkflowsMock.Object,servicioSapMock.Object);

            dto = new RemitoBodegaVinoDto
                {
                    Chofer = new ChoferDto
                        {
                            Id = 2,
                            Nombre = "Walter",
                            NumeroDeDocumento = "39123456"
                        },

                    Material = "Vino Malbec",
                    MaterialId = 1,
                    NroRemito = "32165498",
                    Patente = "WMD001",
                    Transportista = "Test",
                    TransportistaId = 2,
                    TipoComercial = "Tipo Comercial",
                    TipoComercialId = 2,
                    MaterialIdYPosicion = "1|a",
                    Posicion = "a"
                };

            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto
                        {
                            Id = 1,
                            Descripcion = "T1"
                        },
                    new TipoComercialDto
                        {
                            Id = 2,
                            Descripcion = "T2"
                        }

                };

            tipoDeDocumentoDeIdentidad = new List<TipoDocumentoIdentidadDto>
                {
                    new TipoDocumentoIdentidadDto
                        {
                            Id = 1,
                            Descripcion = "TDI1",
                            DescripcionCorta = "TDI1"
                        }
                };

            centroDto = new CentroDto
                {
                    Id = 2,
                    Descripcion = "Centro Vinos",
                    CodigoSAP = "8466"
                };

            srvRepositorio.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto{ TipoDeWorkflow = TipoDeWorkflow.Ingreso, Activo = true, Codigo = "WVino1"});
            srvRepositorio.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(true);
            srvRepositorio.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(centroDto);
            srvRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            srvRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipoDeDocumentoDeIdentidad);
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                          .Returns(new TipoComercialDto {Id = 1, Descripcion = "T1"});

            actividadFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            srvRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>()))
                          .Returns(new List<MaterialPorWorkflowDto> {new MaterialPorWorkflowDto()});
            srvRepositorio.Setup(s => s.ListarTiposVehiculoBodega()).Returns(new List<TipoVehiculoBodegaDto>{new TipoVehiculoBodegaDto()});
            srvRepositorio.Setup(s => s.ListarMaterialesBin(It.IsAny<int>(), It.IsAny<int>(), null))
                          .Returns(new List<TipoBinDto> {new TipoBinDto{Id = 1,Descripcion = "Material 1"}});
            srvRepositorio.Setup(s => s.ObtenerProveedorPorCodigoSap(It.IsAny<string>())).Returns(new ProveedorDto{Id = 1, RazonSocial = "Proveedor", CodigoSap = "13213465498798"});

            contractMock.Setup(
                s =>
                s.RemitoBodegaVino(dto, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(),
                                   It.IsAny<ControlRecorridoDto>()))
                        .Returns(new ResultadoCrearWorkflow {InstanciaWorkflowId = instanciaWorkflowId});
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario {CentroId = 2};
            var result = target.Index("WVino1", datosUsuario) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            IEnumerable<SelectListItem> tiposComercialesViewBag = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDeDocumentoViewBag = target.ViewBag.TiposDocumentos;

            Assert.That(tiposComercialesViewBag.Select(s => s.Text), Is.EquivalentTo(new List<string>{"T1","T2"}));
            Assert.That(tiposDeDocumentoViewBag.Select(s => s.Text), Is.EquivalentTo(new List<string>{"TDI1"}));
            Assert.That((string)target.ViewBag.Workflow, Is.EqualTo("WVino1"));

        }

        [Test]
        public void TestIndexDefinicionInactiva()
        {
            srvRepositorio.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(false);
            var datosUsuario = new DatosUsuario{CentroId = 2};
            var result = target.Index("WVino1", datosUsuario) as RedirectToRouteResult;
            
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);

        }

        [Test]
        public void TestCentroNoSeleccionado()
        {
            var datosUsuario = new DatosUsuario();
            var result = target.Index(workflow, dto, datosUsuario) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            srvComando.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestPatenteExistente()
        {
            listaWorkflowsMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>()))
                              .Returns(new InstanciaWorkflowDto());
            var result = target.Index(workflow,dto, new DatosUsuario {CentroId = 2}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            srvComando.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.PatenteEnOtroWorkflow));
            
        }

        [Test]
        public void TestActualizacionChofer()
        {
            dto.Chofer.Id = 0;
            srvRepositorio.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                          .Returns(new List<ChoferDto> {new ChoferDto {Id = 4, Nombre = "Test", Apellido = "Test"}});

            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto{Id = 2, Descripcion = "Proveedor Test"});
            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 2}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);

            srvComando.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
        }

        [Test]
        public void TestChoferActualizacionError()
        {
            dto.Chofer.Id = 0;
            srvRepositorio.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                          .Returns(new List<ChoferDto> {new ChoferDto {Id = 4, Nombre = "Test", Apellido = "Test"}});
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","Error"));
            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);
            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 2}) as ViewResult;
            srvComando.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage,Is.EqualTo("Error") );
        }

        [Test]
        public void TestChoferNuevo()
        {
            dto.Chofer.Id = 0;
            srvRepositorio.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                          .Returns(new List<ChoferDto>());
            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto{Id = 2, Descripcion = "Proveedor Vinos"});
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 2}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);

            srvComando.Verify(s => s.Ejecutar(It.IsAny<CrearChofer>()), Times.Once());
        }

        [Test]
        public void TestChoferNuevoError()
        {
            dto.Chofer.Id = 0;
            srvRepositorio.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                          .Returns(new List<ChoferDto>());
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","Error"));
            srvComando.Setup(c => c.Ejecutar(It.IsAny<CrearChofer>())).Returns(resultado);
            var result = target.Index(workflow, dto,new DatosUsuario {CentroId = 2}) as ViewResult;
            Assert.NotNull(result);
            srvComando.Verify(c => c.Ejecutar(It.IsAny<CrearChofer>()),Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Error"));

        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportista()
        {
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto{ Id = 2, Descripcion = "T1", TransportistaEsProveedor = true});
            srvRepositorio.Setup(t => t.BuscarChoferes(It.Is<ChoferFiltro>(s => s.Cuil == dto.Chofer.Cuil)))
                          .Returns(new List<ChoferDto>());
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto{Cuil = "20-65696868-1", Descripcion = "Proveedor1", RazonSocial = "Proveedor1"});
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(new ResultadoCrear());

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 3}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            srvComando.Verify(s => s.Ejecutar(It.IsAny<CrearTransportista>()),Times.Once());
        }


        [Test]
        public void TestTransportistaEsProveedorTransportistaExistente()
        {
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto{Id = 2,Descripcion = "T1", TransportistaEsProveedor = true});
            srvRepositorio.Setup(t => t.BuscarChoferes(It.Is<ChoferFiltro>(s => s.Cuil == dto.Chofer.Cuil))).Returns( new List<ChoferDto>());
            srvRepositorio.Setup(s => s.ObtenerTransportistaPorCuit(It.IsAny<string>())).Returns(new TransportistaDto());
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>()))
                          .Returns(new ProveedorDto {Id = 2, Descripcion = "Proveedor 1", RazonSocial = "Proveedor 1"});

            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 2}) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            srvComando.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Never());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));

        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportistaError()
        {
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                          .Returns(new TipoComercialDto {Id = 2, Descripcion = "T1", TransportistaEsProveedor = true});
            srvRepositorio.Setup(c => c.BuscarChoferes(It.Is<ChoferFiltro>(s => s.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            srvRepositorio.Setup(c => c.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto{Cuil = "21-11111111-21", Descripcion = "Proveedor1", RazonSocial = "Proveedor1"});
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","error"));
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(resultado);

            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 2}) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }
        
        [Test]
        public void TestTransportistaObligatorio()
        {
            dto.TransportistaId = 0;
            srvRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            srvRepositorio.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            srvRepositorio.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            srvComando.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(resultado);

            var result = target.Index(workflow,dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.Not.Empty);
        }

        private void ValidarVistaConError(ViewResult result)
        {
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            var model = result.Model as RemitoBodegaVinoDto;
            IEnumerable<SelectListItem> tiposComercialesViewBag = result.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosViewBag = result.ViewBag.TiposDocumentos;

            Assert.That(tiposComercialesViewBag.Select(s => s.Text), Is.EquivalentTo(new List<string>{"T1","T2"}));
            Assert.That(tiposDocumentosViewBag.Select(s => s.Text), Is.EquivalentTo(new List<string>{"TDI1"}));
            Assert.That(target.ViewBag.workflow, Is.EqualTo("WVino1"));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }
    }
}
