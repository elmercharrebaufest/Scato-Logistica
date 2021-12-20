using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearLogTarjetaSupervisorTest
    {
        private ProcesadorCrearLogTarjetaSupervisor target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearLogTarjetaSupervisor(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestCrearMotivo()
        {
            var puesto = new PuestoDeTrabajo {SensorQuiebre = "SENSOR1", Id = 1};

            LogTarjetaSupervisor log = null;
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<int>())).Returns(puesto);
            repositorioMock.Setup(r => r.Agregar(It.IsAny<LogTarjetaSupervisor>()))
                    .Returns<LogTarjetaSupervisor>(x => {
                        log = x;
                        return x;
                    });
            var resultado = target.Ejecutar(new CrearLogTarjetaSupervisor { PuestoDeTrabajoId = 1, NumeroTarjeta = "11111111"});
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogTarjetaSupervisor>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
            Assert.That(log.Fecha.Date, Is.EqualTo(DateTime.Today));
            Assert.That(log.PuestoDeTrabajo.Id, Is.EqualTo(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
