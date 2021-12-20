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
    public class IngresarOrdenCargaInternaControllerTest
    {
        private IngresarOrdenCargaInternaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>> actFactoryMock;
        private Mock<IIngresarOrdenCargaInternaService> contractMock;

        private Mock<IListaDeWorkflows> listaMock;

        private OrdenCargaInternaDto dto;
        private ControlRecorridoDto controlRecorridoDto;
        private List<TipoComercialDto> tiposComerciales;
        private List<MaterialDto> materiales;
        private List<TipoDocumentoIdentidadDto> tipos;

        private NullLogger logger;

        private Guid InstanciaWfId = Guid.NewGuid();

        const string workflow = "W1";

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>>();
            contractMock = new Mock<IIngresarOrdenCargaInternaService>();
            listaMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new IngresarOrdenCargaInternaController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, listaMock.Object);

            dto = new OrdenCargaInternaDto
            {
                Chofer = new ChoferDto { Id = 1, Nombre = "Emilio", Apellido = "Saionz", NumeroDeDocumento = "123"},
                FechaEmision = new DateTime(2010, 1, 1),
                DestinoId = 1,
                MaterialId = 1,
                NumeroOrden = "11",
                PatenteCamion = "AAABBB",
                TipoComercialId = 1,
                TransportistaId = 1,
                
            };

            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto {Id = 1, Descripcion = "D1"},
                    new TipoComercialDto {Id = 2, Descripcion = "D2"}
                };

            controlRecorridoDto = new ControlRecorridoDto
            {
                Actividad = "act",
                ActividadXaml = "act",
                Comentario = "coment",
                Decision = true,
                Id = 1,
                Mensaje = "mensaje",
                NombreUsuario = "usuario",
                PuestoDeTrabajoId = 1,
                Fecha = new DateTime(2010, 1, 1)
            };

            materiales = new List<MaterialDto> { new MaterialDto { Id = 1, Descripcion = "M1" } };
            tipos = new List<TipoDocumentoIdentidadDto> { new TipoDocumentoIdentidadDto {Id = 1, DescripcionCorta = "T1"} };

            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>()))
                .Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ListarTiposDocumentoIdentidad())
                .Returns(tipos);

            servRepositorioMock.Setup(s => s.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>()))
                .Returns(new List<PesoMaximoPorTipoVehiculoDto> { new PesoMaximoPorTipoVehiculoDto { Activo = true, CentroId = 1, Id = 1, PesoMaxEgreso = 1, PesoMaxIngreso = 1, PesoNetoMaxPlanta = 1, TipoVehiculo = TipoVehiculo.Camión } });
            servRepositorioMock.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<MaterialPorWorkflowDto> { new MaterialPorWorkflowDto { Id = 1, WorkflowId = 1, CentroId = 1, MaterialId = 1 , MaterialDesc = "M1"} });
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>()))
                .Returns(new WorkflowDto { TipoDeWorkflow = TipoDeWorkflow.Ingreso, Codigo = "W1", Activo = true });
            servRepositorioMock.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>()))
                .Returns(1);
            servRepositorioMock.Setup(s => s.WorkflowActivoConDefinicionActiva(It.IsAny<string>()))
                .Returns(true);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                .Returns(new List<AlmacenDto>());

            contractMock.Setup(s => s.IngresarOrdenCargaInterna(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<ControlRecorridoDto>()))
                .Returns(new ResultadoCrearWorkflow { InstanciaWorkflowId = InstanciaWfId }); 
        }


        [Test]
        public void TestIndex()
        {
            DatosUsuario datosUsuario = new DatosUsuario() { CentroId = 1 };

            var result = target.Index("W1", datosUsuario) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            IEnumerable<SelectListItem> materiales = target.ViewBag.Materiales;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
        }


        [Test]
        public void TestChoferNoSeleccionadoCuilExistenteDiferenteChofer()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                .Returns(new TipoComercialDto { Id = 1, Descripcion = "D1" ,TransportistaEsProveedor = false});
            contractMock.Setup(s => s.IngresarOrdenCargaInterna(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), controlRecorridoDto))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarChofer>(d => d.Dto == dto.Chofer))).Returns(new Resultado { });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> materialesVb = target.ViewBag.Materiales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(materialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "M1" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestChoferNoSeleccionadoCuilExistenteMismoChofer()
        {
            dto.EsTransportista = true;
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });
            

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }

        [Test]
        public void TestChoferNoSeleccionadoCuilNoExistente()
        {
            dto.EsTransportista = true;
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(new ResultadoCrear { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestChoferNoSeleccionadoCuilNoExistenteConErrores()
        {
            const string workflow = "W1";
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil)))
                .Returns(new List<ChoferDto>());
            contractMock.Setup(s => s.IngresarOrdenCargaInterna(dto, It.IsAny<int>(), workflow, It.IsAny<int>(), It.IsAny<string>(), controlRecorridoDto))
                .Returns(new Resultado());

            var resultado = new ResultadoCrear { Id = 1 };
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(resultado);

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            IEnumerable<SelectListItem> tiposComercialesVb = target.ViewBag.TiposComerciales;
            IEnumerable<SelectListItem> tiposDocumentosVb = target.ViewBag.TiposDocumentos;
            var codigo = (string)target.ViewBag.Workflow;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(codigo, Is.EqualTo("W1"));
            Assert.That(tiposComercialesVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "D1", "D2" }));
            Assert.That(tiposDocumentosVb.Select(s => s.Text), Is.EquivalentTo(new List<string> { "T1" }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void TestChoferSeleccionado()
        {
            dto.EsTransportista = true;
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(new TipoComercialDto { TransportistaEsProveedor = false });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<CrearChofer>(d => d.Dto == dto.Chofer))).Returns(new ResultadoCrear { Id = 1 });
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { new ChoferDto { Id = 4, Nombre = "Test", Apellido = "Test" } });

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());

            var result = target.Index(workflow, dto, new DatosUsuario {CentroId = 1 }) as RedirectToRouteResult;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestPatenteEnOtroWorkflow()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            dto.Chofer.Id = 0;
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.Is<ChoferFiltro>(c => c.Cuil == dto.Chofer.Cuil))).Returns(new List<ChoferDto> { dto.Chofer });
            listaMock.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>())).Returns(new InstanciaWorkflowDto());

            var result = target.Index(workflow, dto, new DatosUsuario{CentroId = 1}) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.PatenteEnOtroWorkflow));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearChofer>()), Times.Never());
        }
    }
}
