using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresoRemitoControllerTest
    {
        private IngresoRemitoController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private Mock<IServicioActividadFactory<IIngresoRemitoService>> factory;
        private Mock<IIngresoRemitoService> contract;
        private DatosUsuario datos;
        private NullLogger log;
        private Mock<IListaDeWorkflows> listaWorkflows;
        private Guid instanceId;
        private string obtenerDatosNro;
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;
        private RemitoDto dto;

        [SetUp]
        public void SetUp()
        {

            dto = new RemitoDto
            {
                OrdenDeDescarga = "77777777",
                TipoComercial = "Tipo Comercial",
                TipoComercialId = 1,
                Transportista = "Test",
                Chofer = new ChoferDto { Id = 1, Nombre = "Bruce", Apellido = "Wayne", NumeroDeDocumento = "123" },
                Material = "Manzanas",
                MaterialId = 1,
                PatenteCamion = "AAA111",
                TransportistaId = 3
            };

            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            listaWorkflows = new Mock<IListaDeWorkflows>();
            datos = new DatosUsuario{NombreUsuario = "w", CentroId = 1};
            factory = new Mock<IServicioActividadFactory<IIngresoRemitoService>>();
            contract = new Mock<IIngresoRemitoService>();
            log = new NullLogger();
            instanceId = Guid.NewGuid();
            target = new IngresoRemitoController(log,servRepositorio.Object,factory.Object,servcomandos.Object,listaWorkflows.Object);
            obtenerDatosNro = "1234-123456789012";
            servRepositorio.Setup(s => s.WorkflowActivoConDefinicionActiva("A")).Returns(true);
            servRepositorio.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>()))
                           .Returns(new WorkflowDto
                               {
                                   Codigo = "w",
                                   CentroId = 1,
                                   Descripcion = "W",
                                   TipoDeWorkflow = TipoDeWorkflow.Ingreso
                               });
            servRepositorio.Setup(s => s.ObtenerNumeroOrdenDeDescargaGenerado(It.IsAny<int>())).Returns("1234");
            servRepositorio.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>()))
                           .Returns(new List<TipoComercialDto>
                               {
                                   new TipoComercialDto {Descripcion = "T", Id = 1, TransportistaEsProveedor = false}
                               });
            servRepositorio.Setup(s => s.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<MaterialPorWorkflowDto>
                               {
                                   new MaterialPorWorkflowDto
                                       {
                                           Id = 1,
                                           MaterialDesc = "M",
                                           MaterialId = 1,
                                           RequiereAnexoInase = true
                                       }
                               });

            servRepositorio.Setup(s => s.ListarTiposDocumentoIdentidad())
                           .Returns(new List<TipoDocumentoIdentidadDto>
                               {
                                   new TipoDocumentoIdentidadDto {Descripcion = "Dni", Id = 1}
                               });
            servRepositorio.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>()))
                           .Returns(new[] {new ChoferDto{Cuil = "12-12345678-1"}});
            servRepositorio.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>()))
                           .Returns(new TipoComercialDto {TransportistaEsProveedor = true, Id = 1, Descripcion = "T"});
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto {Id = 1, Descripcion = "Centro",CodigoSAP = "Centro"});
            servRepositorio.Setup(s => s.ObtenerUltimaWorkflowDefinicionPorCordigo(It.IsAny<string>())).Returns(1);
            servRepositorio.Setup(s => s.ObtenerRecorridoPorNumeroDocumentoSap(It.IsAny<string>()))
                           .Returns(new RecorridoDto
                               {
                                   NumeroDeDocumentoSap = obtenerDatosNro,
                                   InstanciaWorkflow = instanceId,
                                   Chofer = new ChoferDto {Id = 1, Cuil = "12-12345678-1"},
                                   DocumentoInternoSap = "Dc",
                                   PesoBruto = 1000,
                                   PesoTara = 100,
                                   Centro = new CentroDto()
                               });
            servRepositorio.Setup(s => s.ObtenerOrdenEntrePlantasPorInstanceId(It.IsAny<Guid>()))
                           .Returns(new OrdenEntrePlantasDto
                               {
                                   CentroDestinoId = 1,
                                   TipoComercialId = 1,
                                   TransportistaId = 1,
                                   MaterialId = 1,
                                   PatenteCamion = "AAA111"
                               });
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>()))
                           .Returns(new TransportistaDto {Id = 1, RazonSocial = "Tr", Cuit = "12-12345678-22"});
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>()))
                           .Returns(new MaterialDto {Id = 1, Descripcion = "M"});
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s =>
                s.IngresoRemito(It.IsAny<RemitoDto>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(new ResultadoCrearWorkflow{InstanciaWorkflowId = instanceId, Id = 1});
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
        }

        [Test]
        public void IndexWorkflowActivo()
        {
            var result = target.Index("A", datos) as ViewResult;
            Assert.NotNull(result);
            Assert.True(result.ViewBag.EsIngreso);
            Assert.That(result.ViewBag.TiposComercialesTransportista, Is.EquivalentTo(new List<string>{"1"}));
            Assert.That(result.ViewBag.MaterialesConAnexo, Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void IndexWorkflowInactivo()
        {
            var result = target.Index("I", datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void IndexPostSinErrores()
        {
            var result =
                target.Index("A",
                             new RemitoDto
                                 {
                                     Material = "M",
                                     MaterialId = 1,
                                     Remito = "1234-123465789",
                                     EsRemitoProveedor = true,
                                     TransportistaId = 1,
                                     Chofer = new ChoferDto {Cuil = "12-12345678-1"},
                                     EsTransportista = true,
                                     PatenteCamion = "AAA111"
                                 }, datos) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"],"Index");
            Assert.AreEqual(result.RouteValues["controller"],"ListaDeCamiones");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
            Assert.IsEmpty(target.TempData.Keys);
        }

        [Test]
        public void IndexPostDatosUsuarioCentroIdError()
        {
            datos.CentroId = 0;
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = true,
                                 PatenteCamion = "AAA111"
                             }, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(target.TempData.First(s => s.Key == "Alerta").Value, "Debe seleccionar un centro");
            Assert.AreEqual(target.TempData.First(s => s.Key == "TipoAlerta").Value, TipoAlerta.Advertencia);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void IndexPostRemitoExistenteError()
        {
            servRepositorio.Setup(s => s.ObtenerRemitoPorNroRemito(It.IsAny<string>())).Returns(new RemitoDto());
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = true,
                                 PatenteCamion = "AAA111"
                             }, datos) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.First(s => s.Key == "Remito").Value.Errors.FirstOrDefault().ErrorMessage, "Nro de Remito existente");
            Assert.True(result.ViewBag.EsIngreso);
            Assert.That(result.ViewBag.TiposComercialesTransportista, Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(result.ViewBag.MaterialesConAnexo, Is.EquivalentTo(new List<string> { "1" }));

        }

        [Test]
        public void IndexPostPatenteEnOtroWorkflowError()
        {
            listaWorkflows.Setup(s => s.ObtenerWorkflowPorPatente(It.IsAny<string>()))
                          .Returns(new InstanciaWorkflowDto());
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = true,
                                 PatenteCamion = "AAA111"
                             }, datos) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(target.TempData.First(s => s.Key == "Alerta").Value, "La patente ingresada ya existe en otro workflow en ejecución");
            Assert.AreEqual(target.TempData.First(s => s.Key == "TipoAlerta").Value, TipoAlerta.Advertencia);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void IndexPostResultadoChoferFalso()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(resultado);
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = true,
                                 PatenteCamion = "AAA111"
                             }, datos) as ViewResult;
            Assert.NotNull(result);
            Assert.True(result.ViewBag.EsIngreso);
            Assert.That(result.ViewBag.TiposComercialesTransportista, Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(result.ViewBag.MaterialesConAnexo, Is.EquivalentTo(new List<string> { "1" }));
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void IndexPostResultadoTransportistaTrue()
        {
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = false,
                                 PatenteCamion = "AAA111"
                             }, datos) as ViewResult;
            Assert.NotNull(result);
            Assert.True(result.ViewBag.EsIngreso);
            Assert.That(result.ViewBag.TiposComercialesTransportista, Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(result.ViewBag.MaterialesConAnexo, Is.EquivalentTo(new List<string> { "1" }));
        }

        [Test]
        public void IndexPostResultadoErrorEnActividad()
        {
            var resultado = new ResultadoCrearWorkflow();
            resultado.Error("Error", "error");
            contract.Setup(
                s =>
                s.IngresoRemito(It.IsAny<RemitoDto>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);
            var result =
                target.Index("A",
                             new RemitoDto
                             {
                                 Material = "M",
                                 MaterialId = 1,
                                 Remito = "1234-123465789",
                                 EsRemitoProveedor = true,
                                 TransportistaId = 1,
                                 Chofer = new ChoferDto { Cuil = "12-12345678-1" },
                                 EsTransportista = true,
                                 PatenteCamion = "AAA111"
                             }, datos) as ViewResult;
            Assert.NotNull(result);
            Assert.True(result.ViewBag.EsIngreso);
            Assert.That(result.ViewBag.TiposComercialesTransportista, Is.EquivalentTo(new List<string> { "1" }));
            Assert.That(result.ViewBag.MaterialesConAnexo, Is.EquivalentTo(new List<string> { "1" }));
            Assert.AreEqual(result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void ObtenerDatos()
        {

            var result = target.ObtenerDatos(obtenerDatosNro) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ datosDB = Molinos.Scato.Dominio.Dto.RemitoDto }");
        }

        [Test]
        public void ObtenerDatosRemitoInexistente()
        {
            servRepositorio.Setup(s => s.ObtenerOrdenEntrePlantasPorInstanceId(It.IsAny<Guid>()))
                           .Returns((OrdenEntrePlantasDto) null);
            var result = target.ObtenerDatos(obtenerDatosNro) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ datosDB = -3, error = No se encontró ningún Remito existente. }");
        }
        [Test]
        public void ObtenerDatosRemitoErrorVacio()
        {
            servRepositorio.Setup(s => s.ObtenerOrdenEntrePlantasPorInstanceId(It.IsAny<Guid>()))
                           .Returns((OrdenEntrePlantasDto)null);
            var result = target.ObtenerDatos("AAAAAAAAAAAAAA") as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ datosDB = -1, error = Debe ingresar un número de Remito. }");
        }

        [Test]
        public void ObtenerDatosException()
        {
            servRepositorio.Setup(s => s.ObtenerOrdenEntrePlantasPorInstanceId(It.IsAny<Guid>()))
                           .Throws(new Exception("Error"));
            var result = target.ObtenerDatos(obtenerDatosNro) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ datosDB = -1, error = Ha ocurrido un error al intentar obtener los datos del Remito. Por favor intente nuevamente. }");
        }
        
        [Test]
        public void TestChoferEnOtroRecorrido()
        {
            const string workflow = "W1";

            servRepositorio.Setup(s => s.ObtenerOtroRecorridoDelChofer(It.IsAny<int>()))
               .Returns(new OtroRecorridoDelChoferDto { NumeroDocumentoIngreso = "77777777", Patente = "AAA111" });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(string.Format(Textos.Error_ChoferYaEstaEnPlanta, dto.Chofer.NombreCompleto, dto.OrdenDeDescarga, "AAA111")));
        }

        [Test]
        public void TestRemitoExistente()
        {
            const string workflow = "W1";

            servRepositorio.Setup(s => s.ObtenerRemitoPorNroRemito(It.IsAny<string>()))
               .Returns(new RemitoDto { Id = 1 });

            var result = target.Index(workflow, dto, new DatosUsuario { CentroId = 1 }) as ViewResult;
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.IngresarRemito_Existente));
        }
    }
}
