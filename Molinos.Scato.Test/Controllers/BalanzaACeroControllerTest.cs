using System;
using System.Collections.Generic;
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
    public class BalanzaACeroControllerTest
    {
        private BalanzaACeroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IServicioActividadFactory<IBalanzaACeroService>> actFactoryMock;
        private Mock<IBalanzaACeroService> contractMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioNotificarUsuario> servNotificarUsuarioMock;
        private Mock<HttpContextBase> contextMock;

        private NullLogger logger;
        private RecorridoDto recorrido;
        private CentroDto centro;
        private Guid instancia;
        private DatosUsuario datosUsuario;
    
        [SetUp]
        public void SetUp()
        {
            HttpContext.Current = FactoryContext.FakeHttpContext();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            actFactoryMock = new Mock<IServicioActividadFactory<IBalanzaACeroService>>();
            contractMock = new Mock<IBalanzaACeroService>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            servNotificarUsuarioMock = new Mock<IServicioNotificarUsuario>();
            logger = new NullLogger();
            target = new BalanzaACeroController(logger, servRepositorioMock.Object, servComandosMock.Object, servNotificarUsuarioMock.Object, orquestadorMock.Object, actFactoryMock.Object);
            instancia = new Guid("25892e17-80f6-415f-9c65-7395632f0223");
            centro = new CentroDto {Id = 1, Descripcion = "Centro 1"};
            recorrido = new RecorridoDto
                {
                    Id = 1,
                    Almacen = new AlmacenDto {Id = 1, CentroId = 1, Descripcion = "Almacen 1"},
                    Centro = centro,
                    Chofer = new ChoferDto {Id = 1, Nombre = "Chofer"},
                    DatosProximaActividad = "Tara",
                    InstanciaWorkflow = instancia,
                    Material = new MaterialDto {Id = 1, Descripcion = "MaterialDesc 1"},
                    NumeroDocumentoIngreso = "1111",
                    Patente = "AAA111",
                    TipoComercial = new TipoComercialDto {Id = 1, Descripcion = "Tipo 1"},
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                    Workflow = new WorkflowDto {Id = 1, Descripcion = "EgresoMaterialNoProductivo"}
                };
            datosUsuario = new DatosUsuario{CentroId = 1};
            var request = new Mock<HttpRequestBase>();
            var session = new MockHttpSession();
            contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(x => x.Request).Returns(request.Object);
            contextMock.SetupGet(x => x.Session).Returns(session);
        }
        [Test]
        public void TestBalanzaACero()
        {
            var id = 1;
            var result = target.BalanzaACero(id) as PartialViewResult;
            Assert.That(result.ViewName, Is.EqualTo("BalanzaACero"));
            Assert.That((int?)target.ViewBag.Modalidad, Is.EqualTo(id));
        }
        [Test]
        public void TestBalanzaEnCero()
        {
            var balanzaId = 2;
            target.ControllerContext = new ControllerContext(contextMock.Object, new RouteData(), target);
            target.ControllerContext = new ControllerContext(contextMock.Object, new RouteData(), target);
            target.Url = new UrlHelper(target.ControllerContext.RequestContext);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());
            var resultado = target.BalanzaEnCero(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\",\"url\":null}"));
            servComandosMock.Verify(v => v.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(e => e.EstaEnCero)), Times.Once());
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
        }
        [Test]
        public void TestBalanzaEnCeroEnActividad()
        {
            var balanzaId = 2;
            var request = new Mock<HttpRequestBase>();
            var session = new MockHttpSession();
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            context.SetupGet(x => x.Session).Returns(session);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            target.Url = new UrlHelper(target.ControllerContext.RequestContext);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.BalanzaACero(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()));
            var resultado = target.BalanzaEnCero(balanzaId, datosUsuario, It.IsAny<Guid>(), It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\"}"));
            servComandosMock.Verify(v => v.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(e => e.EstaEnCero)), Times.Once());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Once());
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
        }
        [Test]
        public void TestCerearBalanzaCereoPermitidoBalanzaManual()
        {
            var balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto{Modalidad = Modalidad.Manual});
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\"}"));
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Once());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }
        [Test]
        public void TestCerearBalanzaCereoPermitidoBalanzaManualNoExitoso()
        {
            var balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Modalidad = Modalidad.Manual });
            var result = new Resultado();
            result.Errores.Add("error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(result);
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"error\"}"));
            Assert.That(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"], Is.EqualTo(null));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Once());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }
        [Test]
        public void TestCerearBalanzaCereoPermitidoBalanzaAutomaticaCereoExitoso()
        {
            const int balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Modalidad = Modalidad.Automática, MaximoValorCereo = 100});
            var valores = new Dictionary<string, decimal> {{"Pesaje", 50}};
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(new ResultadoEjecutar { Valores = valores, Mensaje = new Mensaje()});
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarCereoCabezal>())).Returns(new ResultadoEjecutar{Mensaje = new Mensaje()});
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\"}"));
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
            servComandosMock.Verify(v => v.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(e => e.EstaEnCero)), Times.Once());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }
        [Test]
        public void TestCerearBalanzaCereoPermitidoBalanzaAutomaticaCereoNoExitoso()
        {
            const int balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Modalidad = Modalidad.Automática, MaximoValorCereo = 100 });
            var valores = new Dictionary<string, decimal> { { "Pesaje", 50 } };
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(new ResultadoEjecutar { Valores = valores, Mensaje = new Mensaje() });
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarCereoCabezal>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje{Codigo = 5, Descripcion = "error"} });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"error\"}"));
            Assert.That(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"], Is.EqualTo(null));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Never());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Once());
        }
        [Test]
        public void TestCerearBalanzaCereoNoPermitidoBalanzaAutomaticaCereo()
        {
            const int balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Modalidad = Modalidad.Automática, MaximoValorCereo = 40 });
            var valores = new Dictionary<string, decimal> { { "Pesaje", 50 } };
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarPesaje>())).Returns(new ResultadoEjecutar { Valores = valores, Mensaje = new Mensaje() });
            orquestadorMock.Setup(s => s.Ejecutar(It.IsAny<EjecutarCereoCabezal>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje() });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, null, null) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"" + Textos.BalanzaACero_ExcedePesoMaximo + "\"}"));
            Assert.That(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"], Is.EqualTo(null));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Never());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Never());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Once());
        }
        [Test]
        public void TestCerearBalanzaCereoPermitidoBalanzaManualEnActividad()
        {
            var balanzaId = 2;
            servRepositorioMock.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto { Modalidad = Modalidad.Manual });
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.BalanzaACero(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()));
            var resultado = target.CerearBalanza(balanzaId, datosUsuario, It.IsAny<Guid>(), It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\"}"));
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Once());
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Once());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }
        [Test]
        public void TestIndex()
        {
            var balanza = new BalanzaDto
                {
                    Modalidad = Modalidad.Manual,
                    Id = 1,
                };
            var guid = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
            recorrido.DatosProximaActividad = "1";
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerBalanza(1)).Returns(balanza);
            var resultado = target.Index(guid, new DatosUsuario()) as ViewResult;
            Assert.That(resultado.ViewBag.Modalidad, Is.EqualTo(balanza.Modalidad));
            Assert.That(resultado.ViewBag.InstanciaWorkflow, Is.EqualTo(guid));
            Assert.That(resultado.ViewBag.BalanzaId, Is.EqualTo(balanza.Id));
            Assert.That(resultado.ViewBag.WorkflowDefinicionId, Is.EqualTo(recorrido.WorkflowDefinicionId));
        }

        [Test]
        public void TestAvanzarBalanzaCereo()
        {
            var balanzaId = 2;
            var comando = new CrearMotivoForzarCero { Motivo = "Motivo", BalanzaId = balanzaId ,Fecha = new DateTime(2015,10,07,10,11,22),InstanceId = new Guid(),Usuario = "manuel"};
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMotivoForzarCero>())).Returns(new ResultadoCrear());
            
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.BalanzaACero(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()));
            var resultado = target.Avanzar(balanzaId, comando.Motivo, datosUsuario, It.IsAny<Guid>(), It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"true\"}"));
            Assert.That(int.Parse(HttpContext.Current.Response.Cookies["datosUsuario"]["BalanzaId"]), Is.EqualTo(balanzaId));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Once());
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearMotivoForzarCero>()), Times.Exactly(1));
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Once());
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }

        [Test]
        public void TestAvanzarBalanzaCereoFallo()
        {
            var balanzaId = 2;
            var resultado2 = new Resultado();
            resultado2.Error("","Error al setear la balanza en cero");

            var comando = new CrearMotivoForzarCero { Motivo = "Motivo", BalanzaId = balanzaId, Fecha = new DateTime(2015, 10, 07, 10, 11, 22), InstanceId = new Guid(), Usuario = "manuel" };
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ModificarBalanzaEstaEnCero>(c => c.EstaEnCero))).Returns(resultado2);
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMotivoForzarCero>())).Returns(new ResultadoCrear());

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            contractMock.Setup(s => s.BalanzaACero(It.IsAny<Guid>(), It.IsAny<ControlRecorridoDto>()));
            var resultado = target.Avanzar(balanzaId, comando.Motivo, datosUsuario, It.IsAny<Guid>(), It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"Error al setear la balanza en cero\"}"));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Once());
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearMotivoForzarCero>()), Times.Exactly(1));
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Exactly(0));
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }

        [Test]
        public void TestAvanzarBalanzaCereoComentarioCorto()
        {
            var balanzaId = 2;
            var comando = new CrearMotivoForzarCero { Motivo = "Mot", BalanzaId = balanzaId, Fecha = new DateTime(2015, 10, 07, 10, 11, 22), InstanceId = new Guid(), Usuario = "manuel" };

            var resultado = target.Avanzar(balanzaId, comando.Motivo, datosUsuario, It.IsAny<Guid>(), It.IsAny<int>()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"data\":\"El motivo debe tener más de 5 caracteres\"}"));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>()), Times.Exactly(0));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearMotivoForzarCero>()), Times.Exactly(0));
            contractMock.Verify(v => v.BalanzaACero(It.IsAny<Guid>(), It.Is<ControlRecorridoDto>(e => e.Actividad == Textos.ActBalanzaACero)), Times.Exactly(0));
            servNotificarUsuarioMock.Verify(v => v.Notificar(It.IsAny<NotificacionDto>()), Times.Never());
        }
    }
}
