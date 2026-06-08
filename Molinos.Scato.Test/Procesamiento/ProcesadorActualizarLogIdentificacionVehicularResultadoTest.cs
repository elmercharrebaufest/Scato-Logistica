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
    public class ProcesadorActualizarLogIdentificacionVehicularResultadoTest
    {
        private ProcesadorActualizarLogIdentificacionVehicularResultado target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarLogIdentificacionVehicularResultado(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestActualizarLogExitoso()
        {
            // Arrange
            var logExistente = new LogIdentificacionVehicular
            {
                Id = 1,
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ001",
                Patente = "ABC123",
                ResultadoWorkflow = null
            };

            repositorioMock.Setup(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()))
                .Returns(logExistente);

            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 1,
                ResultadoWorkflow = "Workflow avanzado exitosamente",
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(logExistente.ResultadoWorkflow, Is.EqualTo("Workflow avanzado exitosamente"));

            repositorioMock.Verify(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestActualizarLogConRecorridoYPuesto()
        {
            // Arrange
            var logExistente = new LogIdentificacionVehicular
            {
                Id = 2,
                CodigoDispositivo = "DISP002",
                ResultadoWorkflow = null
            };

            var recorridoExistente = new Recorrido { Id = 100, Patente = "ABC123" };
            var puestoExistente = new PuestoDeTrabajo { Id = 200, NombrePuesto = "Puesto 1" };

            repositorioMock.Setup(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()))
                .Returns(logExistente);
            repositorioMock.Setup(r => r.Obtener<Recorrido>(100)).Returns(recorridoExistente);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(200)).Returns(puestoExistente);

            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 2,
                ResultadoWorkflow = "Listo para avanzar workflow",
                RecorridoId = 100,
                PuestoDeTrabajoId = 200
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(logExistente.ResultadoWorkflow, Is.EqualTo("Listo para avanzar workflow"));
            Assert.That(logExistente.Recorrido, Is.EqualTo(recorridoExistente));
            Assert.That(logExistente.PuestoDeTrabajo, Is.EqualTo(puestoExistente));

            repositorioMock.Verify(r => r.Obtener<Recorrido>(100), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(200), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestValidacionLogIdInvalido()
        {
            // Arrange
            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 0, // ID inválido
                ResultadoWorkflow = "Test",
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey("LogId"), Is.True);
            Assert.That(resultado.Errores["LogId"], Is.EqualTo("El ID del log es requerido"));

            repositorioMock.Verify(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestValidacionLogIdNegativo()
        {
            // Arrange
            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = -5, // ID negativo
                ResultadoWorkflow = "Test",
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey("LogId"), Is.True);

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestValidacionResultadoWorkflowVacio()
        {
            // Arrange
            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 1,
                ResultadoWorkflow = "", // Vacío
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey("ResultadoWorkflow"), Is.True);
            Assert.That(resultado.Errores["ResultadoWorkflow"], Is.EqualTo("El resultado del workflow es requerido"));

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestValidacionResultadoWorkflowNull()
        {
            // Arrange
            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 1,
                ResultadoWorkflow = null, // Null
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Errores.ContainsKey("ResultadoWorkflow"), Is.True);

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestLogNoEncontrado()
        {
            // Arrange
            repositorioMock.Setup(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()))
                .Returns((LogIdentificacionVehicular)null);

            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 999,
                ResultadoWorkflow = "Test",
                RecorridoId = null,
                PuestoDeTrabajoId = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False); // El método retorna sin error pero no guarda

            repositorioMock.Verify(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Never());
        }

        [Test]
        public void TestActualizarSoloConRecorrido()
        {
            // Arrange
            var logExistente = new LogIdentificacionVehicular
            {
                Id = 3,
                CodigoDispositivo = "DISP003",
                ResultadoWorkflow = null
            };

            var recorridoExistente = new Recorrido { Id = 150, Patente = "XYZ789" };

            repositorioMock.Setup(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()))
                .Returns(logExistente);
            repositorioMock.Setup(r => r.Obtener<Recorrido>(150)).Returns(recorridoExistente);

            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 3,
                ResultadoWorkflow = "Recorrido asignado",
                RecorridoId = 150,
                PuestoDeTrabajoId = null // Sin puesto
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(logExistente.Recorrido, Is.EqualTo(recorridoExistente));
            Assert.That(logExistente.PuestoDeTrabajo, Is.Null);

            repositorioMock.Verify(r => r.Obtener<Recorrido>(150), Times.Once());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<int>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestActualizarSoloConPuesto()
        {
            // Arrange
            var logExistente = new LogIdentificacionVehicular
            {
                Id = 4,
                CodigoDispositivo = "DISP004",
                ResultadoWorkflow = null
            };

            var puestoExistente = new PuestoDeTrabajo { Id = 250, NombrePuesto = "Puesto 2" };

            repositorioMock.Setup(r => r.ObtenerPrimero<LogIdentificacionVehicular>(It.IsAny<System.Linq.Expressions.Expression<System.Func<LogIdentificacionVehicular, bool>>>()))
                .Returns(logExistente);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(250)).Returns(puestoExistente);

            var comando = new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = 4,
                ResultadoWorkflow = "Puesto asignado",
                RecorridoId = null, // Sin recorrido
                PuestoDeTrabajoId = 250
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(logExistente.Recorrido, Is.Null);
            Assert.That(logExistente.PuestoDeTrabajo, Is.EqualTo(puestoExistente));

            repositorioMock.Verify(r => r.Obtener<Recorrido>(It.IsAny<int>()), Times.Never());
            repositorioMock.Verify(r => r.Obtener<PuestoDeTrabajo>(250), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }
    }
}
