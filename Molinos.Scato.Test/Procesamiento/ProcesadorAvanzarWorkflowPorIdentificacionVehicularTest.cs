using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorAvanzarWorkflowPorIdentificacionVehicularTest
    {
        private ProcesadorAvanzarWorkflowPorIdentificacionVehicular target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorAvanzarWorkflowPorIdentificacionVehicular(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestValidacionExitosa()
        {
            // Arrange
            var recorrido = new Recorrido
            {
                Id = 100,
                Patente = "ABC123",
                TarjetaDeAcceso = "TARJ001",
                Terminado = false,
                InstanciaWorkflow = System.Guid.NewGuid()
            };

            var puesto = new PuestoDeTrabajo
            {
                Id = 200,
                NombrePuesto = "Puesto de Entrada",
                CodigoConfigIdentificacionVehicular = "DISP001"
            };

            repositorioMock.Setup(r => r.Obtener<Recorrido>(100)).Returns(recorrido);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(200)).Returns(puesto);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 100,
                PuestoId = 200
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(100), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(200), Times.Once());
        }

        [Test]
        public void TestRecorridoNoEncontrado()
        {
            // Arrange
            repositorioMock.Setup(r => r.Obtener<Recorrido>(999)).Returns((Recorrido)null);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 999,
                PuestoId = 200
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey(nameof(comando.RecorridoId)), Is.True);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(999), Times.Once());
            // No debe intentar buscar el puesto si el recorrido no existe
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void TestPuestoNoEncontrado()
        {
            // Arrange
            var recorrido = new Recorrido
            {
                Id = 100,
                Patente = "ABC123",
                TarjetaDeAcceso = "TARJ001",
                Terminado = false,
                InstanciaWorkflow = System.Guid.NewGuid()
            };

            repositorioMock.Setup(r => r.Obtener<Recorrido>(100)).Returns(recorrido);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(888)).Returns((PuestoDeTrabajo)null);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 100,
                PuestoId = 888
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey(nameof(comando.PuestoId)), Is.True);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(100), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(888), Times.Once());
        }

        [Test]
        public void TestRecorridoYPuestoNoEncontrados()
        {
            // Arrange
            repositorioMock.Setup(r => r.Obtener<Recorrido>(999)).Returns((Recorrido)null);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(888)).Returns((PuestoDeTrabajo)null);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 999,
                PuestoId = 888
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey(nameof(comando.RecorridoId)), Is.True);

            // Solo debe buscar el recorrido, no debe llegar a buscar el puesto
            repositorioMock.Verify(r => r.Obtener<Recorrido>(999), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void TestValidacionConRecorridoTerminado()
        {
            // Arrange
            // Aunque el recorrido esté terminado, el procesador solo valida existencia, no estado
            var recorridoTerminado = new Recorrido
            {
                Id = 100,
                Patente = "ABC123",
                Terminado = true, // Terminado
                InstanciaWorkflow = System.Guid.NewGuid()
            };

            var puesto = new PuestoDeTrabajo
            {
                Id = 200,
                NombrePuesto = "Puesto de Entrada"
            };

            repositorioMock.Setup(r => r.Obtener<Recorrido>(100)).Returns(recorridoTerminado);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(200)).Returns(puesto);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 100,
                PuestoId = 200
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            // El procesador solo valida existencia, no el estado Terminado
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(100), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(200), Times.Once());
        }

        [Test]
        public void TestValidacionConRecorridoConGuIdVacio()
        {
            // Arrange
            var recorridoSinWf = new Recorrido
            {
                Id = 100,
                Patente = "ABC123",
                Terminado = false,
                InstanciaWorkflow = System.Guid.Empty // Guid vacío
            };

            var puesto = new PuestoDeTrabajo
            {
                Id = 200,
                NombrePuesto = "Puesto de Entrada"
            };

            repositorioMock.Setup(r => r.Obtener<Recorrido>(100)).Returns(recorridoSinWf);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(200)).Returns(puesto);

            var comando = new AvanzarWorkflowPorIdentificacionVehicular
            {
                RecorridoId = 100,
                PuestoId = 200
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            // El procesador solo valida existencia, no verifica InstanciaWorkflow
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(100), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(200), Times.Once());
        }
    }
}
