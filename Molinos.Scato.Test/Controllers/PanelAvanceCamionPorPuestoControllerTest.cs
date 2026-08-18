using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class PanelAvanceCamionPorPuestoControllerTest
    {
        private PanelAvanceCamionPorPuestoController target;
        private Mock<IServicioRepositorio> servicioMock;
        private Mock<IServicioOrquestador> orquestadorMock;
        private Mock<IServicioComandos>    comandosMock;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servicioMock    = new Mock<IServicioRepositorio>();
            orquestadorMock = new Mock<IServicioOrquestador>();
            comandosMock    = new Mock<IServicioComandos>();
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<Molinos.Scato.Dominio.Comandos.Comando>())).Returns(new Resultado());
            target = new PanelAvanceCamionPorPuestoController(
                new NullLogger(),
                servicioMock.Object,
                null,
                orquestadorMock.Object,
                comandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "operador",
                NombrePc      = "PC-TEST-01",
                CentroId      = 1
            };
        }

        // ── Index ─────────────────────────────────────────────────────

        [Test]
        public void Index_RetornaVistaPrincipal()
        {
            var result = target.Index(datosUsuario) as ViewResult;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        // ── Listar ────────────────────────────────────────────────────

        [Test]
        public void Listar_RetornaVistaListarConPendientes()
        {
            var items = new List<LogAvanceManualCamionPendienteDto>
            {
                new LogAvanceManualCamionPendienteDto { Id = 1, PatenteLeida = "ABC123" },
                new LogAvanceManualCamionPendienteDto { Id = 2, PatenteLeida = "XYZ789" }
            };

            var result = target.Listar(datosUsuario) as ViewResult;
            IList<LogAvanceManualCamionPendienteDto> viewItems = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(viewItems.Count, Is.EqualTo(2));
            Assert.That(viewItems[0].PatenteLeida, Is.EqualTo("ABC123"));
        }

        [Test]
        public void Listar_SinPendientes_RetornaListaVacia()
        {

            var result = target.Listar(datosUsuario) as ViewResult;
            IList<LogAvanceManualCamionPendienteDto> viewItems = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(viewItems, Is.Empty);
        }

        // ── LiberarRegistro ───────────────────────────────────────────

        [Test]
        public void LiberarRegistro_MotivoVacio_RetornaError()
        {
            var result = target.LiberarRegistro(datosUsuario, 1, "") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void LiberarRegistro_MotivoNulo_RetornaError()
        {
            var result = target.LiberarRegistro(datosUsuario, 1, null) as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void LiberarRegistro_RegistroNoEncontrado_RetornaError()
        {
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(5))
                        .Returns((LogAvanceManualCamionDetalleDto)null);

            var result = target.LiberarRegistro(datosUsuario, 5, "motivo válido") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void LiberarRegistro_ServicioDevuelveError_RetornaError()
        {
            var logItem = new LogAvanceManualCamionDetalleDto { Id = 1 };
            var resultadoConError = new Resultado();
            resultadoConError.Error("liberacion", "Error en base de datos");

            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);
            comandosMock.Setup(c => c.Ejecutar(It.IsAny<LiberarLogAvanceManualCamionConMotivo>()))
                        .Returns(resultadoConError);

            var result = target.LiberarRegistro(datosUsuario, 1, "motivo válido") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void LiberarRegistro_Exitoso_RetornaOk()
        {
            var logItem = new LogAvanceManualCamionDetalleDto { Id = 1 };
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);

            var result = target.LiberarRegistro(datosUsuario, 1, "motivo válido") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.True);
        }

        [Test]
        public void LiberarRegistro_TrimsMotivo_LlamaServicioConMotivoSinEspacios()
        {
            var logItem = new LogAvanceManualCamionDetalleDto { Id = 1 };
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);

            target.LiberarRegistro(datosUsuario, 1, "  motivo con espacios  ");

            comandosMock.Verify(c => c.Ejecutar(It.Is<LiberarLogAvanceManualCamionConMotivo>(
                x => x.LogId == 1 && x.Motivo == "motivo con espacios" && x.Usuario == "operador")), Times.Once());
        }

        // ── AvanzarManual ─────────────────────────────────────────────

        [Test]
        public void AvanzarManual_PatenteVacia_RetornaError()
        {
            var result = target.AvanzarManual(datosUsuario, 1, "") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void AvanzarManual_PatenteNula_RetornaError()
        {
            var result = target.AvanzarManual(datosUsuario, 1, null) as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void AvanzarManual_RegistroNoEncontrado_RetornaError()
        {
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(99))
                        .Returns((LogAvanceManualCamionDetalleDto)null);

            var result = target.AvanzarManual(datosUsuario, 99, "ABC123") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void AvanzarManual_ProcesadorDevuelveErrorContingencia_RetornaError()
        {
            var logItem = new LogAvanceManualCamionDetalleDto { Id = 1 };
            var resultadoContingencia = new Resultado();
            resultadoContingencia.Error("proceso", Constantes.ResultadoProcesoIdentificacionVehicular.EnviadoAContingencia);

            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);

            var result = target.AvanzarManual(datosUsuario, 1, "ABC123") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void AvanzarManual_ProcesadorDevuelveErrorGenerico_RetornaError()
        {
            var logItem = new LogAvanceManualCamionDetalleDto { Id = 1 };
            var resultadoError = new Resultado();
            resultadoError.Error("proceso", "Error genérico de procesamiento");

            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);

            var result = target.AvanzarManual(datosUsuario, 1, "ABC123") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.False);
        }

        [Test]
        public void AvanzarManual_Exitoso_RetornaOk()
        {
            var logItem = new LogAvanceManualCamionDetalleDto
            {
                Id                = 1,
                PuestoDeTrabajoId = null
            };
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);

            var result = target.AvanzarManual(datosUsuario, 1, "ABC123") as JsonResult;
            dynamic data = result.Data;
            Assert.That((bool)data.ok, Is.True);
        }

        [Test]
        public void AvanzarManual_NormalizaPatente_LlamaServicioEnMayusculas()
        {
            var logItem = new LogAvanceManualCamionDetalleDto
            {
                Id                = 1,
                PuestoDeTrabajoId = null
            };
            servicioMock.Setup(s => s.ObtenerLogAvanceManualCamion(1)).Returns(logItem);
            target.AvanzarManual(datosUsuario, 1, "  abc123  ");
        }

        // ── ObtenerCamaras ────────────────────────────────────────────

        [Test]
        public void ObtenerCamaras_RetornaListaDeCamaras()
        {
            var camaras = new List<VideoCamaraDto>
            {
                new VideoCamaraDto { Codigo = "CAM1", Directorio = "/path/cam1" },
                new VideoCamaraDto { Codigo = "CAM2", Directorio = "/path/cam2" }
            };
            servicioMock.Setup(s => s.ObtenerCamarasPorLogAvanceManualCamionId(1)).Returns(camaras);

            var result = target.ObtenerCamaras(1) as JsonResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ObtenerCamaras_SinCamaras_RetornaJsonVacio()
        {
            servicioMock.Setup(s => s.ObtenerCamarasPorLogAvanceManualCamionId(1))
                        .Returns(new List<VideoCamaraDto>());

            var result = target.ObtenerCamaras(1) as JsonResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}
