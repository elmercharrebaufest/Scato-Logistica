using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorProcesarIdentificacionVehicularTest
    {
        private ProcesadorProcesarIdentificacionVehicular target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorProcesarIdentificacionVehicular(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestProcesamientoExitoso()
        {
            // Arrange
            var puesto = new PuestoDeTrabajo
            {
                Id = 100,
                NombrePuesto = "Puesto 1",
                CodigoConfigIdentificacionVehicular = "DISP001"
            };

            var recorrido = new Recorrido
            {
                Id = 200,
                Patente = "ABC123",
                TarjetaDeAcceso = "TARJ001",
                Terminado = false
            };

            repositorioMock.Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock.Setup(r => r.ObtenerPrimero<Recorrido>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recorrido, bool>>>()))
                .Returns(recorrido);

            var comando = new ProcesarIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ001",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Error = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.AvanzarWorkflow, Is.True);
            Assert.That(resultado.RecorridoId, Is.EqualTo(200));
            Assert.That(resultado.PuestoDeTrabajoId, Is.EqualTo(100));

            // Verificar que se llamó a Agregar para el log
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.AtLeastOnce());
        }

        [Test]
        public void TestPuestoNoEncontrado()
        {
            // Arrange
            repositorioMock.Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()))
                .Returns((PuestoDeTrabajo)null);

            var comando = new ProcesarIdentificacionVehicular
            {
                CodigoDispositivo = "DISP_INEXISTENTE",
                Tarjeta = "TARJ001",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Error = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.AvanzarWorkflow, Is.False);
            Assert.That(resultado.ResultadoWorkflow, Is.EqualTo("PuestoNoEncontrado"));
            Assert.That(resultado.RecorridoId, Is.Null);
            Assert.That(resultado.PuestoDeTrabajoId, Is.Null);

            // Se debe crear el log aunque no haya puesto
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
        }

        [Test]
        public void TestSinRecorridoActivo()
        {
            // Arrange
            var puesto = new PuestoDeTrabajo
            {
                Id = 100,
                NombrePuesto = "Puesto 1",
                CodigoConfigIdentificacionVehicular = "DISP001"
            };

            repositorioMock.Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock.Setup(r => r.ObtenerPrimero<Recorrido>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recorrido, bool>>>()))
                .Returns((Recorrido)null);

            var comando = new ProcesarIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ_INEXISTENTE",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Error = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.AvanzarWorkflow, Is.False);
            Assert.That(resultado.ResultadoWorkflow, Is.EqualTo("SinRecorridoActivo"));
            Assert.That(resultado.RecorridoId, Is.Null);
            Assert.That(resultado.PuestoDeTrabajoId, Is.EqualTo(100));

            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
        }

        [Test]
        public void TestConErrorReportadoSinPatente()
        {
            // Arrange
            var comando = new ProcesarIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ001",
                Patente = null, // Sin patente
                VehiculoPresente = false,
                FechaEvento = DateTime.Now,
                Error = "Error de comunicación con cámara",
                Detalles = null
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.AvanzarWorkflow, Is.False);
            Assert.That(resultado.ResultadoWorkflow, Is.EqualTo("Error: Error de comunicación con cámara"));

            // Se debe crear el log con el error
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
            // No debe buscar puesto ni recorrido
            repositorioMock.Verify(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()), Times.Never());
            repositorioMock.Verify(r => r.ObtenerPrimero<Recorrido>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recorrido, bool>>>()), Times.Never());
        }

        [Test]
        public void TestConErrorReportadoPeroConPatente()
        {
            // Arrange
            var puesto = new PuestoDeTrabajo
            {
                Id = 100,
                NombrePuesto = "Puesto 1",
                CodigoConfigIdentificacionVehicular = "DISP001"
            };

            var recorrido = new Recorrido
            {
                Id = 200,
                Patente = "ABC123",
                TarjetaDeAcceso = null,
                Terminado = false
            };

            repositorioMock.Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock.Setup(r => r.ObtenerPrimero<Recorrido>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recorrido, bool>>>()))
                .Returns(recorrido);

            var comando = new ProcesarIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = null,
                Patente = "ABC123", // Tiene patente, debe continuar
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Error = "Error parcial",
                Detalles = null
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.AvanzarWorkflow, Is.True); // Debe avanzar aunque haya error porque hay patente
            Assert.That(resultado.RecorridoId, Is.EqualTo(200));
            Assert.That(resultado.PuestoDeTrabajoId, Is.EqualTo(100));

            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
            repositorioMock.Verify(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<System.Linq.Expressions.Expression<System.Func<PuestoDeTrabajo, bool>>>()), Times.Once());
            repositorioMock.Verify(r => r.ObtenerPrimero<Recorrido>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Recorrido, bool>>>()), Times.AtLeastOnce());
        }
    }
}
