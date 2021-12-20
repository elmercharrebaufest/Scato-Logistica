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
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CargarHojaDeRutaControllerTest
    {
        private CargarHojaDeRutaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<ICargarHojaDeRutaService>> actFactoryMock;
        private Mock<ICargarHojaDeRutaService> contractMock;

        private Mock<IListaDeWorkflows> listaMock;

        private HojaDeRutaDto dto;
        private List<TipoComercialDto> tiposComerciales;
        private List<TipoDocumentoIdentidadDto> tipos;
        private readonly Guid instanciaWfId = Guid.NewGuid();
        private CentroDto centroDto;
        private NullLogger logger;
        private const string workflow = "W1";
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<ICargarHojaDeRutaService>>();
            contractMock = new Mock<ICargarHojaDeRutaService>();
            listaMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new CargarHojaDeRutaController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, listaMock.Object);

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

            dto = new HojaDeRutaDto
            {
                Numero = "77777777",
                TipoComercial = "Tipo Comercial",
                TipoComercialId = 1,
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
                Material = "Manzanas",
                MaterialId = 1,
                PatenteCamion = "AAA111",
                TransportistaId = 3,
            };

            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            centroDto = new CentroDto
            {
                Id = 1,
                CodigoSAP = "0101",
                Descripcion = "Centro1"
            };
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1", Activo = true });
            servRepositorioMock.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(centroDto);
            servRepositorioMock.Setup(s => s.ObtenerNumeroHojaDeRutaGenerado()).Returns(10);
            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial" });
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto() });
            servRepositorioMock.Setup(x => x.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>())).Returns(pesosMax);
            contractMock.Setup(s => s.CargarHojaDeRuta(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = instanciaWfId });
        }

        [Test]
        public void TestIndexDefinicionInactiva()
        {
            servRepositorioMock.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(false);
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index("W1", datosUsuario) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Index("W1", datosUsuario) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            var model = result.Model as HojaDeRutaDto;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> TiposVehiculoVb = target.ViewBag.TiposVehiculo;
            Assert.That(model.Numero, Is.EqualTo(centroDto.CodigoSAP + "-" + "00000010"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That((string)target.ViewBag.Workflow, Is.EqualTo("W1"));
            Assert.That(TiposVehiculoVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "Bitren" }));
        }

        [Test]
        public void TestCentroNoSeleccionado()
        {
            var result = target.Index(workflow, dto, new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestOrdenExistente()
        {
            servRepositorioMock.Setup(s => s.ObtenerHojaDeRutaPorNumero(It.IsAny<string>())).Returns(new HojaDeRutaDto());
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
        }

        [Test]
        public void TestPatenteExistente()
        {
            listaMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto());
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("La patente ingresada ya existe en otro workflow en ejecución"));
        }

        [Test]
        public void TestChoferActualizacion()
        {
            dto.Chofer.Id = 0;
            dto.EsTransportista = true;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
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
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarChofer>()), Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }

        [Test]
        public void TestChoferNuevo()
        {
            dto.EsTransportista = true;
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
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
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(resultado);
            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Once());
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportista()
        {
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(new ResultadoCrear());

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Once());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestTransportistaEsProveedorTransportistaExistente()
        {
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servRepositorioMock.Setup(s => s.ObtenerTransportistaPorCuit(It.IsAny<string>())).Returns(new TransportistaDto());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Never());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestTransportistaEsProveedorNuevoTransportistaError()
        {
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(resultado);

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Once());
        }

        [Test]
        public void TestSinTransportistaObligatorio()
        {
            dto.TransportistaId = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { Id = 1, Descripcion = "Tipo Comercial", TransportistaEsProveedor = true });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearChofer>())).Returns(new ResultadoCrear());
            servRepositorioMock.Setup(s => s.ObtenerProveedor(It.IsAny<int>())).Returns(new ProveedorDto { Cuil = "1-111111-1", Descripcion = "Prov1", RazonSocial = "Prov1", });
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportista>())).Returns(resultado);

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            ValidarVistaConError(result);
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_Requerido, Textos.Transportista)));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearTransportista>()), Times.Never());
        }

        private void ValidarVistaConError(ViewResult result)
        {
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            var model = result.Model as HojaDeRutaDto;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            Assert.That(model.Numero, Is.EqualTo("77777777"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That((string)target.ViewBag.Workflow, Is.EqualTo("W1"));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }
    }
}
