using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarFinMarcaTiempoPorPuestoDeTrabajoTest
    {
        private ProcesadorModificarFinMarcaTiempoPorPuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private PuestoDeTrabajo puesto;
        private MarcaTiempoPorPuestoDeTrabajo registroPendiente;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorModificarFinMarcaTiempoPorPuestoDeTrabajo(
                repositorioMock.Object, conversorMock.Object, log);

            puesto = new PuestoDeTrabajo
            {
                Id = 1,
                NombrePuesto = "Garita Norte",
                ConfigSensores = new ConfigSensores
                {
                    Id = 10,
                    SensorBarreraEntradaArriba = "SLO-SB1-UP",
                    SensorBarreraEntradaAbajo  = "SLO-SB1-DN"
                }
            };

            registroPendiente = new MarcaTiempoPorPuestoDeTrabajo
            {
                Id = 42,
                NumeroDocumento = "DOC001",
                PuestoDeTrabajoId = puesto.Id,
                FechaInicio = DateTime.Now.AddMinutes(-5),
                FechaFin = null
            };
        }

        [Test]
        public void TestActualizar_HappyPath_AsignaFechaFinYGuarda()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(true);
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(registroPendiente);

            var comando = new ModificarFinMarcaTiempoPorPuestoDeTrabajo
            {
                EstadoSensor      = false
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(registroPendiente.FechaFin, Is.Not.Null);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestActualizar_SensorSinPuesto_NoActualiza()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                .Returns((PuestoDeTrabajo)null);

            var comando = new ModificarFinMarcaTiempoPorPuestoDeTrabajo
            {
                CodigoDispositivo = "SENSOR-DESCONOCIDO",
                EstadoSensor      = false
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestActualizar_SinRegistroPendiente_NoActualiza()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(false);

            var comando = new ModificarFinMarcaTiempoPorPuestoDeTrabajo
            {
                CodigoDispositivo = "SLO-SB1-UP",
                EstadoSensor      = true
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestActualizar_PorNumeroDocumento_HappyPath_AsignaFechaFinYGuarda()
        {
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(true);
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(registroPendiente);

            var comando = new ModificarFinMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDocumento = "DOC001",
                EstadoSensor    = false
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(registroPendiente.FechaFin, Is.Not.Null);
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestActualizar_PorNumeroDocumento_SinRegistroPendiente_NoActualiza()
        {
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(false);

            var comando = new ModificarFinMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDocumento = "DOC-INEXISTENTE",
                EstadoSensor    = false
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
