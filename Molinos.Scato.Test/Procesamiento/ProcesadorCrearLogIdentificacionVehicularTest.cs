using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorCrearLogIdentificacionVehicularTest
    {
        private ProcesadorCrearLogIdentificacionVehicular target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearLogIdentificacionVehicular(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestCrearLogConDetalles()
        {
            // Arrange
            var fechaEvento = DateTime.Now;
            var detalles = new List<ResultadoIntentoALPR>
            {
                new ResultadoIntentoALPR
                {
                    CodigoCamara = "CAM001",
                    ProveedorALPR = "Proveedor1",
                    Intentos = 3,
                    Patente = "ABC123",
                    RutaImagen = "/images/test.jpg",
                    Certeza = 0.95m,
                    Exitoso = true
                },
                new ResultadoIntentoALPR
                {
                    CodigoCamara = "CAM002",
                    ProveedorALPR = "Proveedor2",
                    Intentos = 2,
                    Patente = "ABC123",
                    RutaImagen = "/images/test2.jpg",
                    Certeza = 0.88m,
                    Exitoso = false
                }
            };

            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ001",
                Error = null,
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = fechaEvento,
                Detalles = detalles
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado, Is.InstanceOf<ResultadoCrear>());

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.CodigoDispositivo == "DISP001" &&
                       log.Tarjeta == "TARJ001" &&
                       log.Patente == "ABC123" &&
                       log.VehiculoPresente == true &&
                       log.FechaEvento == fechaEvento &&
                       log.ResultadoWorkflow == null
            )), Times.Once());

            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicularDetalle>()), Times.Exactly(2));
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestCrearLogSinDetalles()
        {
            // Arrange
            var fechaEvento = DateTime.Now;
            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP002",
                Tarjeta = "TARJ002",
                Error = null,
                Patente = "XYZ789",
                VehiculoPresente = false,
                FechaEvento = fechaEvento,
                Detalles = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado, Is.InstanceOf<ResultadoCrear>());

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.CodigoDispositivo == "DISP002" &&
                       log.Patente == "XYZ789" &&
                       log.VehiculoPresente == false
            )), Times.Once());

            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicularDetalle>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestCrearLogConError()
        {
            // Arrange
            var fechaEvento = DateTime.Now;
            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP003",
                Tarjeta = "TARJ003",
                Error = "Error de comunicación con cámara",
                Patente = null,
                VehiculoPresente = false,
                FechaEvento = fechaEvento,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.Error == "Error de comunicación con cámara" &&
                       log.Patente == null
            )), Times.Once());

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestCrearLogDetalleExitosoSeCalculaCorrectamente()
        {
            // Arrange
            var detalles = new List<ResultadoIntentoALPR>
            {
                new ResultadoIntentoALPR
                {
                    CodigoCamara = "CAM001",
                    ProveedorALPR = "Proveedor1",
                    Intentos = 1,
                    Patente = "ABC123", // Coincide con la patente del comando
                    RutaImagen = "/test.jpg",
                    Certeza = 0.90m,
                    Exitoso = false // Aunque está false, debería marcarse true por coincidencia de patente
                },
                new ResultadoIntentoALPR
                {
                    CodigoCamara = "CAM002",
                    ProveedorALPR = "Proveedor2",
                    Intentos = 1,
                    Patente = "XYZ789", // No coincide
                    RutaImagen = "/test2.jpg",
                    Certeza = 0.85m,
                    Exitoso = false
                }
            };

            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP004",
                Tarjeta = "TARJ004",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Detalles = detalles
            };

            LogIdentificacionVehicularDetalle detalleGuardado1 = null;
            LogIdentificacionVehicularDetalle detalleGuardado2 = null;

            repositorioMock.Setup(r => r.Agregar(It.IsAny<LogIdentificacionVehicularDetalle>()))
                .Callback<LogIdentificacionVehicularDetalle>(detalle =>
                {
                    if (detalleGuardado1 == null)
                        detalleGuardado1 = detalle;
                    else if (detalleGuardado2 == null)
                        detalleGuardado2 = detalle;
                });

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(detalleGuardado1.Exitoso, Is.True, "El primer detalle debería ser exitoso por coincidencia de patente");
            Assert.That(detalleGuardado2.Exitoso, Is.False, "El segundo detalle debería ser no exitoso por no coincidir la patente");
        }

        [Test]
        public void TestCrearLogDetallesConDetallesVacios()
        {
            // Arrange
            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP005",
                Tarjeta = "TARJ005",
                Patente = "DEF456",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                Detalles = new List<ResultadoIntentoALPR>() // Lista vacía
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);

            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicularDetalle>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }
    }
}
