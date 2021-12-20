using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ControlDeIngresoControllerTest
    {
        private ControlDeIngresoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IControlDeIngresoService>> actFactoryMock;
        private Mock<IControlDeIngresoService> contractMock;
        private OrdenDeDescargaFasonDto dto;

        private IList<TipoComercialDto> tiposComerciales;
        private List<MaterialPorWorkflowDto> materiales;
        private List<TipoDocumentoIdentidadDto> tiposDocumento;

        private NullLogger logger;

        private RecorridoDto recorridoDto;
        private DatosUsuario datosUsuario;
        private const string workflow = "W1";
        private List<PesoMaximoPorTipoVehiculoDto> pesosMax;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IControlDeIngresoService>>();
            logger = new NullLogger();
            target = new ControlDeIngresoController(logger,servRepositorioMock.Object,servComandosMock.Object,actFactoryMock.Object);
            contractMock = new Mock<IControlDeIngresoService>();

            datosUsuario = new DatosUsuario
                {
                    CentroId = 1,
                    NombrePc = "BFWS11111",
                    NombreUsuario = "wandino"
                };

            dto = new OrdenDeDescargaFasonDto
                {
                    Chofer =
                        new ChoferDto
                            {
                                Nombre = "Chofer",
                                Apellido = "A",
                                Cuil = "1234",
                                Id = 1,
                                NumeroDeDocumento = "1234",
                                TipoDocumentoIdentidadId = 1,
                                TipoDocumentoIdentidadCodigoSap = "1"
                            },
                    Id = 1,
                    Material = "M1",
                    MaterialId = 1,
                    FechaOD = new DateTime(2015, 6, 3),
                    NumeroRemito = "11111111",
                    EsTransportista = false,
                    PatenteCamion = "AAA111",
                    TipoComercialId = 1,
                    TipoComercial = "T1",
                    TransportistaId = 1,
                    Procedencia = "Procedencia1",
                    ClienteId = 1,
                    Cliente = "Cliente1"
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

            recorridoDto = new RecorridoDto
                {
                    Id = 1,
                    NumeroDocumentoIngreso = "123",
                    InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                    Centro = new CentroDto{Id = 1, Descripcion = "Centro1"},
                    Workflow = new WorkflowDto{Activo = true, Id = 1, Descripcion = "A",CentroId = 1, Codigo = workflow},
                    WorkflowDefinicionId = 1,
                    Patente = "AAA111"
                    
                };

            

            materiales = new List<MaterialPorWorkflowDto>
                {
                    new MaterialPorWorkflowDto
                        {
                            Id = 1 , 
                            MaterialDesc = "M1"
                           
                        },
                    new MaterialPorWorkflowDto
                        {
                            Id = 2,
                            MaterialDesc = "M2"
                        }
                        
                };

            tiposDocumento = new List<TipoDocumentoIdentidadDto>
                {
                    new TipoDocumentoIdentidadDto
                        {
                            Id = 1,
                            CodigoSap = "1",
                            Descripcion = "D1"
                        }
                };

            servRepositorioMock.Setup(x => x.ListarTiposComercialesPorWfCodigo(It.IsAny<string>()))
                               .Returns(tiposComerciales);
            servRepositorioMock
                .Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(materiales);
            servRepositorioMock.Setup(x => x.ListarTiposDocumentoIdentidad()).Returns(tiposDocumento);
            servRepositorioMock.Setup(c => c.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorridoDto);
            servRepositorioMock.Setup(
                r => r.RecorridoSinPesosPorNumeroDeDocumento(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>()))
                               .Returns(recorridoDto);

            servRepositorioMock.Setup(s => s.ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(It.IsAny<string>()))
                               .Returns(dto);
            servRepositorioMock.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto { Cuit = "112" });
            servRepositorioMock.Setup(s => s.ObtenerProveedorPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>()))
                               .Returns(new ProveedorDto { Activo = true, Id = 1, Descripcion = "P1" });
            tiposComerciales = new List<TipoComercialDto>
                {
                    new TipoComercialDto
                        {
                            CodigoSap = "1",
                            Id = 1,
                            Descripcion = "T1",
                            TransportistaEsProveedor = true
                        },
                    new TipoComercialDto
                        {
                            CodigoSap = "2",
                            Id = 2,
                            Descripcion = "T2",
                            TransportistaEsProveedor = true
                        }

                };

            servRepositorioMock.Setup(s => s.ListarTiposComercialesPorWfCodigo(It.IsAny<string>())).Returns(tiposComerciales);
            servRepositorioMock.Setup(s => s.ObtenerTipoComercial(It.IsAny<int>())).Returns(tiposComerciales.First());
            servRepositorioMock.Setup(x => x.ListarPesoMaximoPorTipoVehiculoPorCentro(It.IsAny<int>())).Returns(pesosMax);
        }

        [Test]
        public void IndexTest()
        {
            
            var result = target.Index(recorridoDto.InstanciaWorkflow) as ViewResult;
            IEnumerable<SelectListItem> TiposVehiculoVb = target.ViewBag.TiposVehiculo;
            
            Assert.AreEqual(result.ViewName, "OrdenDeDescargaFason");
            Assert.IsEmpty(target.ViewBag.TiposComercialesTransportista);
            Assert.That(((List<SelectListItem>)target.ViewBag.TiposComerciales).Select(s => s.Text),Is.EquivalentTo(new List<string>{"T1","T2"}));
            Assert.That(((List<SelectListItem>)target.ViewBag.Materiales).Select(s => s.Text), Is.EquivalentTo(new List<string>{"M1", "M2"}));
            Assert.That(TiposVehiculoVb.Select(s => s.Value), Is.EquivalentTo(new List<string> { "Bitren" }));
        }

        [Test]
        public void TestRechazar()
        {
            List<MotivoDto> motivos = new List<MotivoDto>
                {
                    new MotivoDto
                        {
                            Descripcion = "Motivo1",
                            Id = 1,
                            DescripcionCorta = "Mot1"
                        },
                    new MotivoDto
                        {
                            Descripcion = "Motivo2",
                            Id = 2,
                            DescripcionCorta = "Mot2"
                        }
                };

            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(motivos);

            var result = target.Rechazar(recorridoDto.Workflow.Codigo, recorridoDto.WorkflowDefinicionId,
                                         recorridoDto.InstanciaWorkflow, datosUsuario) as ViewResult;

            Assert.AreEqual(result.ViewName, "_TransportistaRechazado");
            Assert.That(((List<SelectListItem>)target.ViewBag.Motivos).Select(s => s.Text), Is.EquivalentTo(new List<string>{"Motivo1", "Motivo2"}));
            
        }

        [Test]
        public void TransportistaRechazadoTest()
        {
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>()))
                          .Returns(contractMock.Object);
            contractMock.Setup(s => s.ControlDeIngreso(It.IsAny<OrdenDeDescargaFasonDto>(), It.IsAny<bool>(), It.IsAny<ControlRecorridoDto>(), It.IsAny<Guid>())).Returns(new Resultado());
            

            var result = target.TransportistaRechazado(recorridoDto.Workflow.Codigo,recorridoDto.WorkflowDefinicionId,new ControlRecorridoDto{WorkflowInstanceId = recorridoDto.InstanciaWorkflow,Id = 1, Fecha = dto.FechaOD}) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }

        [Test]
        public void OrdenDeDescargaFasonTest()
        {
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>())).Returns(new List<ChoferDto>{dto.Chofer});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarChofer>())).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerWorkflowPorCodigo(It.IsAny<string>()))
                               .Returns(recorridoDto.Workflow);
            var result = target.OrdenDeDescargaFason(recorridoDto.Workflow.Codigo, dto, recorridoDto.InstanciaWorkflow,
                                                     datosUsuario, dto.Id) as ViewResult;

            Assert.NotNull(result);
        }

    }
}
