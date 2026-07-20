using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Firmware;
using Molinos.Scato.Web.ServicioHub;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Firmware
{
    [TestFixture]
    public class FirmwareGaritaIngresoTest
    {
        private Mock<IServicioRepositorio> servicioMock;
        private Mock<IListaDeWorkflows> workflowsMock;
        private Mock<IServicioComandos> comandosMock;
        private Mock<IServicioOrquestador> servicioOrquestadorMock;
        private Mock<IServicioActividadFactory<IEjecutarService>> factoryMock;
        private Mock<IRecorridoWorkflow> recorridoWorkflowMock;
        private Mock<HubClientFactory> hubClientFactoryMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servicioMock = new Mock<IServicioRepositorio>();
            workflowsMock = new Mock<IListaDeWorkflows>();
            comandosMock = new Mock<IServicioComandos>();
            servicioOrquestadorMock = new Mock<IServicioOrquestador>();
            factoryMock = new Mock<IServicioActividadFactory<IEjecutarService>>();
            recorridoWorkflowMock = new Mock<IRecorridoWorkflow>();
            hubClientFactoryMock = new Mock<HubClientFactory>(MockBehavior.Loose);
            hubClientFactoryMock.Setup(f => f.GetClient(It.IsAny<string>())).Returns((HubClient)null);
            hubClientFactoryMock.Setup(f => f.GetClientNotificar(It.IsAny<string>())).Returns((HubClientNotificar)null);

            recorridoWorkflowMock
                .Setup(r => r.ObtenerRecorrido(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new DatosRecorridoDto { SinRecorrido = true });
        }

        // --- Ciclo 8: Tarjeta válida con cámaras → ejecuta dispositivos y notifica ---

        [Test]
        public void ProcesarEvento_TarjetaValidaConCamaras_EjecutaDispositivosYNotifica()
        {
            var firmware = CrearFirmware();
            var lectura = CrearLectura(tarjetaValida: true, conCamaras: true);
            lectura.PrimerNumeroDeTarjeta = lectura.NumeroDeTarjeta; // primera lectura → notifica

            firmware.ProcesarEvento(lectura);

            // EjecutarDispositivos se verifica a través de ObtenerRecorrido
            recorridoWorkflowMock.Verify(r => r.ObtenerRecorrido(
                lectura.NumeroDeTarjeta, lectura.PuestoDeTrabajoId), Times.Once());
            // NotificarLecturaPorSignalR se verifica a través del override de InvokeNotificarLectura
            Assert.That(firmware.LecturasNotificadas, Has.Count.EqualTo(1));
        }

        // --- Ciclo 9: Tarjeta inválida → no ejecuta dispositivos, sí notifica ---

        [Test]
        public void ProcesarEvento_TarjetaInvalida_NoEjecutaDispositivosPeroNotifica()
        {
            var firmware = CrearFirmware();
            var lectura = CrearLectura(tarjetaValida: false, conCamaras: true);

            firmware.ProcesarEvento(lectura);

            recorridoWorkflowMock.Verify(r => r.ObtenerRecorrido(
                It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            Assert.That(firmware.LecturasNotificadas, Has.Count.EqualTo(1));
        }

        // --- Ciclo 10: Tarjeta válida sin cámaras → no ejecuta dispositivos, sí notifica ---

        [Test]
        public void ProcesarEvento_TarjetaValidaSinCamaras_NoEjecutaDispositivosPeroNotifica()
        {
            var firmware = CrearFirmware();
            var lectura = CrearLectura(tarjetaValida: true, conCamaras: false);
            lectura.PrimerNumeroDeTarjeta = lectura.NumeroDeTarjeta; // primera lectura → notifica

            firmware.ProcesarEvento(lectura);

            recorridoWorkflowMock.Verify(r => r.ObtenerRecorrido(
                It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            Assert.That(firmware.LecturasNotificadas, Has.Count.EqualTo(1));
        }

        // --- Ciclo 11: Segunda lectura diferente tarjeta → ejecuta dispositivos, no notifica ---

        [Test]
        public void ProcesarEvento_SegundaLecturaDiferenteTarjeta_NoNotifica()
        {
            var firmware = CrearFirmware();
            var lectura = CrearLectura(tarjetaValida: true, conCamaras: true);
            lectura.PrimerNumeroDeTarjeta = "OTRA_TARJETA"; // diferente → no notifica
            lectura.NumeroDeTarjeta = "123456";

            firmware.ProcesarEvento(lectura);

            recorridoWorkflowMock.Verify(r => r.ObtenerRecorrido(
                lectura.NumeroDeTarjeta, lectura.PuestoDeTrabajoId), Times.Once());
            Assert.That(firmware.LecturasNotificadas, Is.Empty);
        }

        // --- Helpers ---

        private FirmwareGaritaIngresoTestable CrearFirmware()
        {
            return new FirmwareGaritaIngresoTestable(
                log,
                servicioMock.Object,
                workflowsMock.Object,
                comandosMock.Object,
                servicioOrquestadorMock.Object,
                factoryMock.Object,
                recorridoWorkflowMock.Object,
                hubClientFactoryMock.Object);
        }

        private LecturaPuestoDeTrabajoDto CrearLectura(bool tarjetaValida, bool conCamaras)
        {
            return new LecturaPuestoDeTrabajoDto
            {
                PuestoDeTrabajoId = 10,
                CentroId = 1,
                NumeroDeTarjeta = "123456",
                PrimerNumeroDeTarjeta = string.Empty,
                TarjetaValida = tarjetaValida,
                Entrada = new List<string>(),
                Salida = new List<string>(),
                VideoCamaras = conCamaras
                    ? new List<VideoCamaraDto> { new VideoCamaraDto { Id = 1, Codigo = "CAM-01" } }
                    : new List<VideoCamaraDto>()
            };
        }

        // Subclase testable que expone las notificaciones enviadas por SignalR
        private class FirmwareGaritaIngresoTestable : FirmwareGaritaIngreso
        {
            public List<LecturaPuestoDeTrabajoDto> LecturasNotificadas { get; } = new List<LecturaPuestoDeTrabajoDto>();

            public FirmwareGaritaIngresoTestable(
                Ninject.Extensions.Logging.ILogger log,
                IServicioRepositorio servicioRepositorio,
                IListaDeWorkflows workflows,
                IServicioComandos comandos,
                IServicioOrquestador servicioOrquestador,
                IServicioActividadFactory<IEjecutarService> factory,
                IRecorridoWorkflow recorridoWorkflow,
                HubClientFactory hubClientFactory)
                : base(log, servicioRepositorio, workflows, comandos,
                       servicioOrquestador, factory, recorridoWorkflow, hubClientFactory)
            {
            }

            protected override void InvokeNotificarLectura(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo)
            {
                LecturasNotificadas.Add(lecturaPuestoDeTrabajo);
            }
        }
    }
}
