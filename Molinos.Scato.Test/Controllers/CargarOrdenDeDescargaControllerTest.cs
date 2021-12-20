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
    public class CargarOrdenDeDescargaControllerTest
    {
        private CargarOrdenDeDescargaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<ICargarOrdenDeDescargaService>> actFactoryMock;
        private Mock<ICargarOrdenDeDescargaService> contractMock;

        private Mock<IListaDeWorkflows> listaMock;

        private OrdenDeDescargaDto dto;
        private List<TipoComercialDto> tiposComerciales;
        private List<TipoDocumentoIdentidadDto> tipos;
        private List<WorkflowDto> workflows;
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;

        private NullLogger logger;

        private Guid InstanciaWfId = Guid.NewGuid();

        const string workflow = "W1";

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<ICargarOrdenDeDescargaService>>();
            contractMock = new Mock<ICargarOrdenDeDescargaService>();
            listaMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new CargarOrdenDeDescargaController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, listaMock.Object);

            dto = new OrdenDeDescargaDto()
            {
                Numero = "77777777",
                FechaMovimiento = DateTime.Now.AddDays(2),
                TipoComercial = "TipoComercialDesc",
                Proveedor = "Test",
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123", Cuil = "123"},
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

            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto { Id = 1, DescripcionCorta = "T1" } };
            workflows = new List<WorkflowDto> { new WorkflowDto() { Id = 1, Codigo = "W1", Descripcion = "W1" } };

            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad()).Returns(tipos);

            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>())).Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1", Activo = true});
            servRepositorioMock.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            servRepositorioMock.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>())).Returns(true);
            servRepositorioMock.Setup(x => x.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>())).Returns(pesosMax);

            contractMock.Setup(s => s.CargarOrdenDeDescarga(It.IsAny<OrdenDeDescargaDto>(), It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = InstanciaWfId });
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto{CodigoSAP = "1111"});
            DatosUsuario datosUsuario = new DatosUsuario() { CentroId = 1 };
            var result = target.Index("W1", datosUsuario) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> TiposVehiculoVb = target.ViewBag.TiposVehiculo;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(TiposVehiculoVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "Bitren" }));
        }

        [Test]
        public void TestChoferNoSeleccionado()
        {
            dto.EsTransportista = true;
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>())).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Juan", Apellido = "Perez" } });
            var resultado = new Resultado();
            resultado.Errores.Add("error","");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto{TransportistaEsProveedor = false});

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ViewBag.EsIngreso, Is.EqualTo(true));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestChoferSeleccionado()
        {
            dto.EsTransportista = true;
            
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>())).Returns(new List<ChoferDto> ());

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(new ResultadoCrear { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
        
        [Test]
        public void TestCentroNoSeleccionado()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });

            var result = target.Index(workflow, dto, new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestOrdeDeCargaContenedorExistente()
        {
            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaPorNumeroDeOrden(It.IsAny<string>()))
               .Returns(new OrdenDeDescargaDto { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.OrdenDeDescarga_Existente));
        }

        [Test]
        public void TestChoferEnOtroRecorrido()
        {
            servRepositorioMock.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
               .Returns(new OtroRecorridoDelChoferDto { NumeroDocumentoIngreso = "77777777", Patente = "AAA111" });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_ChoferYaEstaEnPlanta, dto.Chofer.NombreCompleto, dto.Numero, "AAA111")));
        }
    }
}
