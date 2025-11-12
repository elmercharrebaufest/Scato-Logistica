using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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
    public class ProcesadorModificarMonitoreoServicioExternoTest
    {
        private ProcesadorModificarMonitoreoServicioExterno target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarMonitoreoServicioExterno(repositorioMock.Object, conversorMock.Object, new NullLogger());
        }

        [Test]
        public void TestModificarEntidad()
        {
            var entidad = new MonitoreoServicioExterno
            {
                Id = 1,
                UltimoEstado = HealthCheckStatus.Desconocido,
                UltimaVerificacion = DateTime.UtcNow.AddMinutes(-10)
            };
            var comando = new ModificarMonitoreoServicioExterno
            {
                Id = 1,
                UltimoEstado = HealthCheckStatus.Conectado,
                UltimaVerificacion = DateTime.UtcNow
            };
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<MonitoreoServicioExterno, bool>>>())).Returns(true);
            repositorioMock.Setup(r => r.Obtener<MonitoreoServicioExterno>(It.IsAny<int>())).Returns(entidad);

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Exactly(1));
            Assert.IsFalse(resultado.HayErrores);
            Assert.AreEqual(comando.UltimoEstado, entidad.UltimoEstado);
            Assert.AreEqual(comando.UltimaVerificacion, entidad.UltimaVerificacion);
        }

        [Test]
        public void TestModificarEntidadInvalidoNoExistePorId()
        {
            var comando = new ModificarMonitoreoServicioExterno
            {
                Id = 1,
                UltimoEstado = HealthCheckStatus.Desconocido,
                UltimaVerificacion = DateTime.UtcNow
            };
            repositorioMock.Setup(r => r.Existe(It.IsAny<Expression<Func<MonitoreoServicioExterno, bool>>>())).Returns(false);

            var resultado = target.Ejecutar(comando);

            Assert.IsTrue(resultado.HayErrores);
        }
    }
}
