using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class PesadaControllerTest
    {
        private PesadaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IPesadaService>> actFactoryMock;
        private Mock<IPesadaService> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IListaDeWorkflows> workflowMock;
        private NullLogger logger;
        private RecorridoDto recorrido;
        private CentroDto centro;
        private Guid instancia;
        private List<BalanzaDto> balanzas;
        private List<AlmacenDto> almacenes;
        private List<PuestosDeCargaDescargaDto> hidraulicas;
        private List<CalleDto> calles;
        private DatosUsuario datosUsuario;
        private ControlRecorridoDto controlRecorridoDto;
        private List<MotivoDto> motivos;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IPesadaService>>();
            contractMock = new Mock<IPesadaService>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            workflowMock = new Mock<IListaDeWorkflows>();
            logger = new NullLogger();
            target = new PesadaBrutoController(logger, servRepositorioMock.Object, actFactoryMock.Object, servComandosMock.Object, orquestadorMock.Object, workflowMock.Object);
            instancia = new Guid("25892e17-80f6-415f-9c65-7395632f0223");

            centro = new CentroDto { Id = 1, Descripcion = "Centro 1" };

            recorrido = new RecorridoDto
            {
                Id = 1,
                Almacen = new AlmacenDto { Id = 1, CentroId = 1, Descripcion = "Almacen 1" },
                Centro = centro,
                Chofer = new ChoferDto { Id = 1, Nombre = "Chofer" },
                DatosProximaActividad = "Tara",
                InstanciaWorkflow = instancia,
                Material = new MaterialDto { Id = 1, Descripcion = "MaterialDesc 1" },
                NumeroDocumentoIngreso = "1111",
                Patente = "AAA111",
                TipoComercial = new TipoComercialDto { Id = 1, Descripcion = "Tipo 1" },
                TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                Workflow = new WorkflowDto { Id = 1, Descripcion = "EgresoMaterialNoProductivo" }
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

            balanzas = new List<BalanzaDto>
                {
                    new BalanzaDto
                        {
                            Id = 1,
                            Nombre = "Balanza1",
                            EstaEnCero = false,
                            Modalidad = Modalidad.Manual,
                            Color = "#FFFFFF",
                            PuestoDeTrabajo = "U1"
                        },
                    new BalanzaDto
                        {
                            Id = 2,
                            Nombre = "Balanza2",
                            EstaEnCero = false,
                            Modalidad = Modalidad.Automática,
                            Color = "#FFFFFF",
                            PuestoDeTrabajo = "U2"
                        },
                };

            hidraulicas = new List<PuestosDeCargaDescargaDto>
                {
                    new PuestosDeCargaDescargaDto
                        {
                            Id = 1,
                            Nombre = "Balanza1",
                            CentroId = 1,
                            Codigo = "codigo",
                            EsSojaSustentable = false,
                            PuestoDeTrabajoId = 1,
                            PuestoDeTrabajo = "U1"
                        },
                    new PuestosDeCargaDescargaDto
                        {
                            Id = 2,
                            Nombre = "Balanza2",
                            CentroId = 1,
                            Codigo = "codigo2",
                            EsSojaSustentable = false,
                            PuestoDeTrabajoId = 2,
                            PuestoDeTrabajo = "U2"
                        },
                };

            calles = new List<CalleDto>
                {
                    new CalleDto
                        {
                            Id = 1,
                            Nombre = "Balanza1",
                            CentroId = 1,
                            Codigo = "codigo"

                        },
                    new CalleDto
                        {
                            Id = 2,
                            Nombre = "Balanza2",
                            CentroId = 1,
                            Codigo = "codigo2"
                        },
                };
            motivos = new List<MotivoDto>
                {
                    new MotivoDto
                        {
                            Id = 1,
                            Descripcion = "motivo1",
                            DescripcionCorta = "U1"
                        },
                    new MotivoDto
                        {
                            Id = 2,
                             Descripcion = "motivo2",
                            DescripcionCorta = "U2"
                        },
                };
            almacenes = new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "Almacen 1", CentroId = 1 } };

            datosUsuario = new DatosUsuario { CentroId = 1, CentroDescripcion = "CENTROtest", NombreUsuario = "NOMBREtest", PuestoDeTrabajoId = 1, NombrePc = "U1", BalanzaId = balanzas[0].Id };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarCalles(1)).Returns(new List<CalleDto>());
            servRepositorioMock.Setup(s => s.ListarHidraulicas(It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<PuestosDeCargaDescargaDto>());
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(motivos);

            var result = target.Index(It.IsAny<Guid>(), datosUsuario) as ViewResult;
            var model = (Pesada)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("~/Views/Pesada/Index.cshtml"));
            Assert.That(((List<SelectListItem>)target.ViewBag.Balanzas).Select(s => s.Text), Is.EquivalentTo(balanzas.Select(s => s.Nombre)));
            Assert.That(((List<SelectListItem>)target.ViewBag.Almacenes).Select(s => s.Text), Is.EquivalentTo(almacenes.Select(s => s.DescripcionCorta)));
            Assert.That(((List<SelectListItem>)target.ViewBag.Motivos).Select(s => s.Text), Is.EquivalentTo(motivos.Select(s => s.Descripcion)));
            Assert.That((string)target.ViewBag.NumeroDocumentoIngreso, Is.EqualTo(recorrido.NumeroDocumentoIngreso));
            Assert.That((TipoDocumentoIngreso)target.ViewBag.DocumentoIngreso, Is.EqualTo(recorrido.TipoDocumentoIngreso));
            Assert.That((string)target.ViewBag.Material, Is.EqualTo(recorrido.Material.Descripcion));
            Assert.That((int?)target.ViewBag.PesoBrutoOrigen, Is.EqualTo(recorrido.PesoBrutoOrigen));
            Assert.That((int?)target.ViewBag.PesoTaraOrigen, Is.EqualTo(recorrido.PesoTaraOrigen));
            Assert.That((int?)target.ViewBag.PesoBruto, Is.EqualTo(recorrido.PesoBruto));
            Assert.That((int?)target.ViewBag.PesoTara, Is.EqualTo(recorrido.PesoTara));
            Assert.That(model.WorkflowInstanceId, Is.EqualTo(instancia));
            Assert.That(model.AlmacenId, Is.EqualTo(recorrido.Almacen.Id));
            Assert.That(model.BalanzaId, Is.EqualTo(datosUsuario.BalanzaId));
            Assert.That(model.Patente, Is.EqualTo(recorrido.Patente));
        }

        [Test]
        public void TestIndexReingresaPatente()
        {
            recorrido.Centro.ReingresaPatenteAlPesar = true;
            servRepositorioMock.Setup(s => s.ListarCalles(1)).Returns(new List<CalleDto>());
            servRepositorioMock.Setup(s => s.ListarHidraulicas(It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<PuestosDeCargaDescargaDto>());
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarMotivos()).Returns(motivos);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterialYCentro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            var result = target.Index(It.IsAny<Guid>(), datosUsuario) as ViewResult;
            var model = (Pesada)result.Model;
            Assert.That(result.ViewName, Is.EqualTo("~/Views/Pesada/Index.cshtml"));
            Assert.That(model.Patente, Is.EqualTo(null));
        }

        [Test]
        public void TestIndexPostPatenteIncorrecta()
        {
            var model = new Pesada { WorkflowInstanceId = instancia, Patente = "BBB222", PatenteOriginal = recorrido.Patente, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "U1" };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            var result = target.Index(model, "TestWorkflowTestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1, NombrePc = "U1" }) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Pesada_PatenteInvalida));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
        }

        [Test]
        public void TestIndexPostDatosIncorrectos()
        {
            var model = new Pesada { WorkflowInstanceId = instancia, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "U1" };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            target.ModelState.AddModelError("", "");
            var result = target.Index(model, "TestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1, NombrePc = "U1" }) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.Pesada_Error));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void TestIndexPuestoDeTrabajoError()
        {
            var model = new Pesada { WorkflowInstanceId = instancia, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "XXXX", Peso = 222, ProximaBalanzaId = 2, Patente = "AAA111", AlmacenId = 3, PatenteOriginal = "AAA111" };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            balanzas[0].PuestoDeTrabajo = "ZZZ";
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            var result = target.Index(model, "TestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1, NombrePc = "U1" }) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.Pesada_BalanzaNoCero));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestIndexPostBalanzaEnCero()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));

            var model = new Pesada { WorkflowInstanceId = instancia, Patente = recorrido.Patente, PatenteOriginal = recorrido.Patente, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "U1", Peso = 1000 };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            balanzas[0].EstaEnCero = false;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Pesada(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);

            var result = target.Index(model, "TestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1, NombrePc = "U1" }) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.Pesada_BalanzaNoCero));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Exactly(0));
        }

        [Test]
        public void TestIndexPostPuestoError()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));

            var model = new Pesada { WorkflowInstanceId = instancia, Patente = recorrido.Patente, PatenteOriginal = recorrido.Patente, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "U1", Peso = 1000 };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            balanzas[0].EstaEnCero = false;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Pesada(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);

            var result = target.Index(model, "TestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1, NombrePc = "U2" }) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.Pesada_BalanzaNoCero));
            contractMock.Verify(v => v.Pesada(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<ControlRecorridoDto>()), Times.Never());
        }

        [Test]
        public void TestIndexPostBalanzaError()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));

            var model = new Pesada { WorkflowInstanceId = instancia, Patente = recorrido.Patente, PatenteOriginal = recorrido.Patente, TipoPesada = TipoPesada.Tara, BalanzaPuestoDeTrabajo = "U1", Peso = 1000 };
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión)).Returns(balanzas);
            servRepositorioMock.Setup(s => s.ListarBalanzasPorNombrePc(It.IsAny<int>(), It.IsAny<string>(), TipoVehiculo.Camión)).Returns(balanzas);
            balanzas[0].EstaEnCero = false;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>())).Returns(almacenes);
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.Pesada(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<ControlRecorridoDto>())).Returns(resultado);

            var result = target.Index(model, "TestWorkflow", 1, new DatosUsuario { PuestoDeTrabajoId = 1 }) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(Textos.Pesada_BalanzaNoCero));
            contractMock.Verify(v => v.Pesada(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<ControlRecorridoDto>()), Times.Never());
        }
        [Test]
        public void TestValidarPatenteOk()
        {
            var resultado = target.ValidarPatente(recorrido.Patente, recorrido.Patente) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"resultado\":\"OK\",\"url\":\"\"}"));
        }

        [Test]
        public void TestValidarPatenteError()
        {
            var request = new Mock<HttpRequestBase>();
            var session = new MockHttpSession();

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            context.SetupGet(x => x.Session).Returns(session);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            target.Url = new UrlHelper(target.ControllerContext.RequestContext);
            var resultado = target.ValidarPatente(recorrido.Patente, string.Empty) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"resultado\":\"ERROR\",\"url\":null}"));
        }

        [Test]
        public void TestObtenerBalanza()
        {
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            var resultado = target.ObtenerBalanza(It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"Color\":\"#FFFFFF\",\"Modalidad\":0,\"EstaEnCero\":false,\"PuestoDeTrabajo\":\"U1\"}"));
        }

        [Test]
        public void TomarPeso()
        {
            const int peso = 1000;
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 }, Valores = new Dictionary<string, decimal> { { "Pesaje", peso } } };
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);

            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { NombrePc = "U1" }) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output, Is.EqualTo(peso.ToString(CultureInfo.InvariantCulture)));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(null));
        }

        [Test]
        public void TomarPesoPuestoError()
        {
            const int peso = 1000;
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 }, Valores = new Dictionary<string, decimal> { { "Pesaje", peso } } };
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);

            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { }) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output, Is.EqualTo("1000"));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(null));
        }


        //[Test]
        //public void TomarPesoBalanzaError()
        //{
        //    const int peso = 1000;
        //    var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 }, Valores = new Dictionary<string, decimal> { { "Pesaje", peso } } };
        //    servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
        //    orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);

        //    var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { NombrePc = "U2"}) as JsonResult;
        //    var serializer = new JavaScriptSerializer();
        //    var output = serializer.Serialize(json.Data);
        //    Assert.That(output, Is.EqualTo("\"" + Textos.Pesada_ErrorBalanzaPuestoDeTrabajo + "\""));
        //    Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(null));
        //}


        [Test]
        public void TomarPesoErrorOrquestadorDesconectado()
        {
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Throws(new Exception());

            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { NombrePc = "U1" }) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            Assert.That(output.Contains(Textos.Pesada_AutomaticaError), Is.EqualTo(true));
        }


        [Test]
        public void TomarPesoOrquestadorCodigoError()
        {
            var resultado = new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 555, Descripcion = "Error" } };
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(balanzas[0]);
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(resultado);

            var json = target.TomarPeso(It.IsAny<int>(), new DatosUsuario { NombrePc = "U1" }) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(json.Data);
            orquestadorMock.Verify(v => v.Ejecutar(It.IsAny<EjecutarPesaje>()), Times.Exactly(5));
            Assert.That(output.Contains(Textos.Error), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Codigo.ToString(CultureInfo.InvariantCulture)), Is.EqualTo(true));
            Assert.That(output.Contains(resultado.Mensaje.Descripcion), Is.EqualTo(true));
        }


        [Test]
        public void TomarPesoManual()
        {
            var pesada = new Pesada { Peso = 1000 };
            var content = target.TomarPeso(pesada) as ContentResult;
            Assert.That(content.Content, Is.EqualTo(pesada.Peso.ToString()));
        }

        [Test]
        public void TomarPesoManualError()
        {
            var pesada = new Pesada();
            target.TomarPeso(pesada);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.Keys.Contains("peso"), Is.EqualTo(true));
        }







    }
}
