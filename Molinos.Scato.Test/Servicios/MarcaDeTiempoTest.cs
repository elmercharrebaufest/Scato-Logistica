using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class MarcaDeTiempoTest
    {
        private MarcaDeTiempo target;
        private Mock<IRepositorio> repositorioMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            log = new NullLogger();
            target = new MarcaDeTiempo(repositorioMock.Object, log);
        }

        private void SetupSensor(string codigoSensor, int puestoId, TipoSensorMarcaTiempo tipo)
        {
            repositorioMock
                .Setup(r => r.Obtener<SensorMarcaTiempoPorPuestoDeTrabajo>(
                    It.IsAny<Expression<Func<SensorMarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(new SensorMarcaTiempoPorPuestoDeTrabajo
                {
                    CodigoSensor = codigoSensor,
                    PuestoDeTrabajoId = puestoId,
                    TipoSensor = tipo
                });
        }

        private void SetupUltimoRegistro(MarcaTiempoPorPuestoDeTrabajo registro)
        {
            repositorioMock
                .Setup(r => r.ObtenerMasReciente<MarcaTiempoPorPuestoDeTrabajo>(
                    It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>(),
                    It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, DateTime>>>()))
                .Returns(registro);
        }

        // --- RegistrarPorSensor: sensor Inicio ---

        [Test]
        public void RegistrarPorSensor_SensorInicio_SinRegistroAbierto_CreaRegistro()
        {
            SetupSensor("SENSOR-01", 42, TipoSensorMarcaTiempo.Inicio);
            SetupUltimoRegistro(null);

            target.RegistrarPorSensor("SENSOR-01");

            repositorioMock.Verify(r => r.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void RegistrarPorSensor_SensorInicio_ConRegistroAbierto_NoCreaDuplicado()
        {
            var registroAbierto = new MarcaTiempoPorPuestoDeTrabajo
            {
                PuestoDeTrabajoId = 42,
                FechaInicio = DateTime.Now.AddMinutes(-5)
            };
            SetupSensor("SENSOR-01", 42, TipoSensorMarcaTiempo.Inicio);
            SetupUltimoRegistro(registroAbierto);

            target.RegistrarPorSensor("SENSOR-01");

            repositorioMock.Verify(r => r.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void RegistrarPorSensor_SensorInicio_ConRegistroCerrado_CreaRegistroNuevo()
        {
            SetupSensor("SENSOR-01", 42, TipoSensorMarcaTiempo.Inicio);
            SetupUltimoRegistro(null); // el filtro excluye registros con FechaFin, el repo devuelve null

            target.RegistrarPorSensor("SENSOR-01");

            repositorioMock.Verify(r => r.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        // --- RegistrarPorSensor: sensor Fin ---

        [Test]
        public void RegistrarPorSensor_SensorFin_ConRegistroAbierto_CierraRegistro()
        {
            var registroAbierto = new MarcaTiempoPorPuestoDeTrabajo
            {
                PuestoDeTrabajoId = 42,
                FechaInicio = DateTime.Now.AddMinutes(-10)
            };
            SetupSensor("SENSOR-02", 42, TipoSensorMarcaTiempo.Fin);
            SetupUltimoRegistro(registroAbierto);

            target.RegistrarPorSensor("SENSOR-02");

            Assert.That(registroAbierto.FechaFin, Is.Not.Null);
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void RegistrarPorSensor_SensorFin_SinRegistroAbierto_NoHaceNada()
        {
            SetupSensor("SENSOR-02", 42, TipoSensorMarcaTiempo.Fin);
            SetupUltimoRegistro(null);

            target.RegistrarPorSensor("SENSOR-02");

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void RegistrarPorSensor_SensorFin_RegistroYaCerrado_NoHaceNada()
        {
            var registroCerrado = new MarcaTiempoPorPuestoDeTrabajo
            {
                PuestoDeTrabajoId = 42,
                FechaInicio = DateTime.Now.AddMinutes(-10),
                FechaFin = DateTime.Now.AddMinutes(-2)
            };
            SetupSensor("SENSOR-02", 42, TipoSensorMarcaTiempo.Fin);
            SetupUltimoRegistro(registroCerrado);

            target.RegistrarPorSensor("SENSOR-02");

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        // --- RegistrarPorSensor: sensor no configurado ---

        [Test]
        public void RegistrarPorSensor_SensorDesconocido_NoHaceNada()
        {
            repositorioMock
                .Setup(r => r.Obtener<SensorMarcaTiempoPorPuestoDeTrabajo>(
                    It.IsAny<Expression<Func<SensorMarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns((SensorMarcaTiempoPorPuestoDeTrabajo)null);

            target.RegistrarPorSensor("SENSOR-DESCONOCIDO");

            repositorioMock.Verify(r => r.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }
    }
}
