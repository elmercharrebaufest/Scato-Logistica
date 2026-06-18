using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Interfaces;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorRegistrarMarcaDeTiempoTest
    {
        private ProcesadorRegistrarMarcaDeTiempo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IMarcaDeTiempo> marcaDeTiempoMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock  = new Mock<IRepositorio>();
            conversorMock    = new Mock<IConversor>();
            marcaDeTiempoMock = new Mock<IMarcaDeTiempo>();
            log = new NullLogger();

            target = new ProcesadorRegistrarMarcaDeTiempo(
                repositorioMock.Object,
                conversorMock.Object,
                log,
                marcaDeTiempoMock.Object);
        }

        [Test]
        public void Ejecutar_TipoInicio_LlamaRegistrarInicioPorSensor()
        {
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo = TipoRegistroMarcaDeTiempo.Inicio,
                CodigoDispositivo = "SENSOR-01"
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarInicioPorSensor("SENSOR-01"), Times.Exactly(1));
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void Ejecutar_TipoIdentificacion_TriggerLectura_LlamaRegistrarIdentificacion()
        {
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo              = TipoRegistroMarcaDeTiempo.Identificacion,
                PuestoDeTrabajoId = 10,
                NumeroDeTarjeta   = "T001",
                Patente           = "ABC123",
                Trigger           = TipoIdentificacionPorPuesto.IngresoPorLectura
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarIdentificacion(10, "T001", "ABC123", TipoIdentificacionPorPuesto.IngresoPorLectura), Times.Exactly(1));
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void Ejecutar_TipoIdentificacion_TriggerPatente_LlamaRegistrarIdentificacion()
        {
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo              = TipoRegistroMarcaDeTiempo.Identificacion,
                PuestoDeTrabajoId = 10,
                NumeroDeTarjeta   = null,
                Patente           = "ABC123",
                Trigger           = TipoIdentificacionPorPuesto.IngresoPorPatente
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarIdentificacion(10, null, "ABC123", TipoIdentificacionPorPuesto.IngresoPorPatente), Times.Exactly(1));
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void Ejecutar_TipoFin_ConInstanceId_LlamaRegistrarFinPorInstanciaWorkflow()
        {
            var instanceId = Guid.NewGuid();
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo       = TipoRegistroMarcaDeTiempo.Fin,
                InstanceId = instanceId
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarFinPorInstanciaWorkflow(instanceId, It.IsAny<int?>()), Times.Exactly(1));
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void Ejecutar_TipoFin_SinInstanceIdNiDispositivo_DevuelveError()
        {
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo = TipoRegistroMarcaDeTiempo.Fin
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarFinPorInstanciaWorkflow(It.IsAny<Guid>(), It.IsAny<int?>()), Times.Exactly(0));
            marcaDeTiempoMock.Verify(m => m.RegistrarFinPorSensor(It.IsAny<string>()), Times.Exactly(0));
            Assert.That(resultado.HayErrores, Is.True);
        }

        [Test]
        public void Ejecutar_TipoFin_ConCodigoDispositivo_LlamaRegistrarFinPorSensor()
        {
            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo              = TipoRegistroMarcaDeTiempo.Fin,
                CodigoDispositivo = "SENSOR-02"
            };

            var resultado = target.Ejecutar(comando);

            marcaDeTiempoMock.Verify(m => m.RegistrarFinPorSensor("SENSOR-02"), Times.Exactly(1));
            marcaDeTiempoMock.Verify(m => m.RegistrarFinPorInstanciaWorkflow(It.IsAny<Guid>(), It.IsAny<int?>()), Times.Exactly(0));
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void Ejecutar_TipoInicio_ExcepcionEnFachada_DevuelveError()
        {
            marcaDeTiempoMock
                .Setup(m => m.RegistrarInicioPorSensor(It.IsAny<string>()))
                .Throws(new InvalidOperationException("DB error"));

            var comando = new RegistrarMarcaDeTiempo
            {
                Tipo = TipoRegistroMarcaDeTiempo.Inicio,
                CodigoDispositivo = "SENSOR-01"
            };

            var resultado = target.Ejecutar(comando);

            Assert.That(resultado.HayErrores, Is.True);
        }
    }
}
