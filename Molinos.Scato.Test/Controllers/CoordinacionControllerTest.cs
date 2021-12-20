using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class CoordinacionControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private CoordinacionController target;
        private Guid instanceId;
        private Mock<IServicioActividadFactory<ICoordinacionService>> factory;
        private Mock<ICoordinacionService> contract;
        private Mock<IConfiguracionProvider> configuracion;
        private NullLogger log;
        private CentroDto centro;
        private CaladoDto calado;
        private Mock<IListaDeWorkflows> servworkflows;

        [SetUp]
        public void SetUp()
        {
            servcomandos = new Mock<IServicioComandos>();
            servRepositorio = new Mock<IServicioRepositorio>();
            factory = new Mock<IServicioActividadFactory<ICoordinacionService>>();
            contract = new Mock<ICoordinacionService>();
            configuracion = new Mock<IConfiguracionProvider>();
            instanceId = Guid.NewGuid();
            log = new NullLogger();
            servworkflows = new Mock<IListaDeWorkflows>();
            target = new CoordinacionController(log, servRepositorio.Object, factory.Object, configuracion.Object, servworkflows.Object);

            centro = new CentroDto { HorarioDesde = 12, HorarioHasta = 13 };

            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto());
            servRepositorio.Setup(
                s => s.ListarPaginadoAnalisisYCaladoPorCaracteristica(It.IsAny<Guid>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<AnalisisPorCaracteristicaDto>(
                                   new List<AnalisisPorCaracteristicaDto> {new AnalisisPorCaracteristicaDto()}, 1, 1, 1));

            servRepositorio.Setup(s => s.ListarAnalisisYCaladoPorCaracteristicaNoAceptables(It.IsAny<Guid>()))
                           .Returns(new List<AnalisisPorCaracteristicaDto>
                               {
                                   new AnalisisPorCaracteristicaDto {AnalisisDeCalidadId = 1}
                               });
            servRepositorio.Setup(s => s.ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(It.IsAny<Guid>()))
                          .Returns(new List<AnalisisPorCaracteristicaDto>
                              {
                                   new AnalisisPorCaracteristicaDto {AnalisisDeCalidadId = 1}
                              });
            servRepositorio.Setup(s => s.ListarMotivos())
                           .Returns(new List<MotivoDto> {new MotivoDto {Descripcion = "Motivo", Id = 1}});
            servRepositorio.Setup(s => s.ListarCamaras())
                           .Returns(new List<CamaraDto> {new CamaraDto {Id = 1, Descripcion = "Cam"}});
            servRepositorio.Setup(s => s.ObtenerMaterialPorCentroPorInstanceId(It.IsAny<Guid>()))
                           .Returns(new MaterialPorCentroDto{CamaraId = 1});
            servRepositorio.Setup(s => s.CaladoPorCaracteristicaQueSeEnvianACamara(It.IsAny<int>()))
                           .Returns(new List<int> {1});
            servRepositorio.Setup(s => s.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>()))
                           .Returns(new CartaPorteDto { NroCartaPorte = "123456789012", Id = 1 });
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Centro = new CentroDto {Id = 1},
                                   Patente = "AAA111",
                                   TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                                   NumeroDocumentoIngreso = "123456789012"
                               });
            servRepositorio.Setup(s => s.ListarCaracteristicasDeCalidadPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<CaracteristicaDeCalidadDto>
                               {
                                   new CaracteristicaDeCalidadDto
                                       {
                                           SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                                           Id = 1,
                                           Descripcion = "Caracteristica",
                                           SeEnviaACamara = false
                                       }
                               });
            servRepositorio.Setup(s => s.ObtenerVehiculoPorGuid(It.IsAny<Guid>()))
                           .Returns(new VehiculoDto {NumeroVehiculo = 123245564});
            servRepositorio.Setup(s => s.ObtenerCamara(It.IsAny<int>()))
                           .Returns(new CamaraDto {Id = 1,FormatoDeArchivo = CamaraFormatoDeArchivo.BahiaBlanca});
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s =>
                s.Coordinacion(It.IsAny<Guid>(), It.IsAny<DecisionCoordinacion>(), It.IsAny<ControlRecorridoDto>(),
                               It.IsAny<MuestraEnvioACamaraDto>())).Returns(new Resultado());
            servRepositorio.Setup(
                s =>
                    s.ListarCaracteristicasDeCalidadPorMaterialSinExceptuadas(It.IsAny<Guid>(), It.IsAny<int>(),
                        It.IsAny<int>())).Returns(new List<CaracteristicaDeCalidadDto>
                               {
                                   new CaracteristicaDeCalidadDto
                                       {
                                           SituacionEnvioACamara = EnvioACamara.EnCoordinacion,
                                           Id = 1,
                                           Descripcion = "Caracteristica"
                                       }
                               });

            configuracion.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "TiemposCuposOtorgados", "5" } });

            calado = new CaladoDto
            {
                Comentario = "asd"
            };

        }

        [Test]
        public void Index()
        {
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto { Id = 1, Centro = centro, Calado = calado });
            servRepositorio.Setup(s => s.ObtenerInformacionCartaPorte(It.IsAny<int>()))
                           .Returns(new InfoCaladoDto { AgenteCompras = "agente", CTG ="ctg", Cupo ="asad", Entregador ="entregador", EnvioDirectoCamara = false, RtteComercial ="trre", TitularCartaPorte ="titular", TrigoEspecial = false});
            var result = target.Index(instanceId) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.False(result.ViewBag.EsAceptable);
        }

        [Test]
        public void Listar()
        {
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                              .Returns(new RecorridoDto { Id = 1, Centro = centro, Calado = calado });
            servRepositorio.Setup(s => s.ObtenerInformacionCartaPorte(It.IsAny<int>()))
                           .Returns(new InfoCaladoDto { AgenteCompras = "agente", CTG = "ctg", Cupo = "asad", Entregador = "entregador", EnvioDirectoCamara = false, RtteComercial = "trre", TitularCartaPorte = "titular", TrigoEspecial = false });

            var result = target.Listar(instanceId) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Listar");
            Assert.False(result.ViewBag.EsAceptable);
        }

        [Test]
        public void Rechazar()
        {
            var result = target.Rechazar("w", 1, instanceId, new DatosUsuario()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "_TransportistaRechazado");
            Assert.That(((IEnumerable<SelectListItem>)result.ViewBag.Motivos).Select(s => s.Value), Is.EquivalentTo(new List<string>{"Motivo"}));
        }

        [Test]
        public void Recalar()
        {
            var result = target.Recalar("w", 1, instanceId, new DatosUsuario()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "_TransportistaRecalar");
            Assert.That(((IEnumerable<SelectListItem>)result.ViewBag.Motivos).Select(s => s.Value), Is.EquivalentTo(new List<string> { "Motivo" }));
        }

        [Test]
        public void EnviarACamara()
        {
            var result = target.EnviarACamara("w", 1, instanceId,1,1, new DatosUsuario()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "_TransportistaEnviaACamara");
            Assert.That(((IEnumerable<SelectListItem>)result.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "Cam" }));
        }

        [Test]
        public void Aceptar()
        {
            var result = target.Aceptar("W", 1, instanceId, new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void AceptarConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            contract.Setup(
                s =>
                s.Coordinacion(It.IsAny<Guid>(), It.IsAny<DecisionCoordinacion>(), It.IsAny<ControlRecorridoDto>(),
                               It.IsAny<MuestraEnvioACamaraDto>())).Returns(resultado);
            var result = target.Aceptar("W", 1, instanceId, new DatosUsuario()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage,"error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
        }

        [Test]
        public void TransportistaRechazado()
        {
            var result = target.TransportistaRechazado(new ControlRecorridoDto{WorkflowInstanceId = instanceId}, "W", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TransportistaRechazadoConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            contract.Setup(
                s =>
                s.Coordinacion(It.IsAny<Guid>(), It.IsAny<DecisionCoordinacion>(), It.IsAny<ControlRecorridoDto>(),
                               It.IsAny<MuestraEnvioACamaraDto>())).Returns(resultado);
            var result = target.TransportistaRechazado(new ControlRecorridoDto{WorkflowInstanceId = instanceId}, "W", 1) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
        }

        [Test]
        public void TransportistaRecalar()
        {
            var result = target.TransportistaRecalar(new ControlRecorridoDto { WorkflowInstanceId = instanceId }, 1, "W") as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TransportistaRecalarConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            contract.Setup(
                s =>
                s.Coordinacion(It.IsAny<Guid>(), It.IsAny<DecisionCoordinacion>(), It.IsAny<ControlRecorridoDto>(),
                               It.IsAny<MuestraEnvioACamaraDto>())).Returns(resultado);
            var result = target.TransportistaRecalar(new ControlRecorridoDto { WorkflowInstanceId = instanceId }, 1, "W") as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
        }

        [Test]
        public void TransportistaEnviarACamara()
        {
            var result =
                target.TransportistaEnviarACamara(
                    new MuestraEnvioACamaraDto
                    {
                        NroDocumento = "123456789012",
                        WorkflowInstanceId = instanceId
                    }, "W", 1, new DatosUsuario()) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void TransportistaEnviarACamaraConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            contract.Setup(
                s =>
                s.Coordinacion(It.IsAny<Guid>(), It.IsAny<DecisionCoordinacion>(), It.IsAny<ControlRecorridoDto>(),
                               It.IsAny<MuestraEnvioACamaraDto>())).Returns(resultado);
            var result =
                target.TransportistaEnviarACamara(
                    new MuestraEnvioACamaraDto
                        {
                            NroDocumento = "123456789012",
                            WorkflowInstanceId = instanceId
                        }, "W", 1, new DatosUsuario()) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
        }

        [Test]
        public void MensajeEnviandoACamara()
        {
            servRepositorio.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto
            {
                TipoComercial = new TipoComercialDto(),
                Workflow = new WorkflowDto(),
                Calado = new CaladoDto
                {
                    WorkflowInstanceId = new Guid(),
                    CicloDeCalado = 1,
                    MuestraConjunto = 10,
                    CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>(),
                    FechaCreacion = DateTime.Now
                },
                Material = new MaterialDto(),
                Centro = new CentroDto
                {
                    Descripcion = "1243358d",
                    HorarioDesde = DateTime.Now.AddHours(-2).Hour,
                    HorarioHasta = DateTime.Now.AddHours(4).Hour
                }
            }
            );
            servRepositorio.Setup(x => x.ObtenerInformacionCartaPorte(It.IsAny<int>())).Returns(new InfoCaladoDto
            {
                EnvioDirectoCamara = true
            });
            servRepositorio.Setup(x => x.ListarCaracteristicasDeCalidadPorMaterialWorkflow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<CaracteristicaDeCalidadDto>
            {
                new CaracteristicaDeCalidadDto()
            });            
            var resultado = target.Index(new Guid(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()) as ViewResult;
            var envioDirectoCamara = (string)target.ViewBag.EnvioDirectoACamara;

            Assert.NotNull(envioDirectoCamara);
        }

        [Test]
        public void SinMensajeEnviandoACamaraSinEnvioDirectoACamara()
        {
            servRepositorio.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto
            {
                TipoComercial = new TipoComercialDto(),
                Workflow = new WorkflowDto(),
                Calado = new CaladoDto
                {
                    WorkflowInstanceId = new Guid(),
                    CicloDeCalado = 1,
                    MuestraConjunto = 10,
                    CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>(),
                    FechaCreacion = DateTime.Now
                },
                Material = new MaterialDto(),
                Centro = new CentroDto
                {
                    Descripcion = "1243358d",
                    HorarioDesde = DateTime.Now.AddHours(-2).Hour,
                    HorarioHasta = DateTime.Now.AddHours(-1).Hour
                }
            }
            );
            servRepositorio.Setup(x => x.ObtenerInformacionCartaPorte(It.IsAny<int>())).Returns(new InfoCaladoDto
            {
                EnvioDirectoCamara = false
            });
            servRepositorio.Setup(x => x.ListarCaracteristicasDeCalidadPorMaterialWorkflow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<CaracteristicaDeCalidadDto>
            {
                new CaracteristicaDeCalidadDto()
            });
            var resultado = target.Index(new Guid(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()) as ViewResult;
            var envioDirectoCamara = (string)target.ViewBag.EnvioDirectoACamara;

            Assert.Null(envioDirectoCamara);
        }

        [Test]
        public void SinMensajeEnviandoACamaraDentroDeRangoHorario()
        {
            servRepositorio.Setup(x => x.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto
            {
                TipoComercial = new TipoComercialDto(),
                Workflow = new WorkflowDto(),
                Calado = new CaladoDto
                {
                    WorkflowInstanceId = new Guid(),
                    CicloDeCalado = 1,
                    MuestraConjunto = 10,
                    CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>(),
                    FechaCreacion = DateTime.Now
                },
                Material = new MaterialDto(),
                Centro = new CentroDto
                {
                    Descripcion = "1243358d",
                    HorarioDesde = DateTime.Now.AddHours(2).Hour,
                    HorarioHasta = DateTime.Now.AddHours(4).Hour
                }
            }
            );
            servRepositorio.Setup(x => x.ObtenerInformacionCartaPorte(It.IsAny<int>())).Returns(new InfoCaladoDto
            {
                EnvioDirectoCamara = true
            });
            servRepositorio.Setup(x => x.ListarCaracteristicasDeCalidadPorMaterialWorkflow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<CaracteristicaDeCalidadDto>
            {
                new CaracteristicaDeCalidadDto()
            });
            var resultado = target.Index(new Guid(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()) as ViewResult;
            var envioDirectoCamara = (string)target.ViewBag.EnvioDirectoACamara;

            Assert.Null(envioDirectoCamara);
        }

        [Test]
        public void MarcadoEnviadoACamaraAutomatico()
        {

            var result = target.EnviarACamara("w", 1, instanceId, 1, 1, new DatosUsuario()) as ViewResult;
            var seEnviaACamara = (result.Model as MuestraEnvioACamaraDto).CaracteristicasDeCalidad.Select(x => x.SeEnviaACamara).First();
            Assert.NotNull(result);
            Assert.IsTrue(seEnviaACamara);
        }
    }
}
