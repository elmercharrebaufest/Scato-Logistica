using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
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
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<IServicioRepositorio> servicioRepositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            servicioComandosMock = new Mock<IServicioComandos>();
            servicioRepositorioMock = new Mock<IServicioRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearLogIdentificacionVehicular(repositorioMock.Object, conversor, new NullLogger(), servicioComandosMock.Object, servicioRepositorioMock.Object);
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
                ErrorDispositivo = null,
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = fechaEvento,
                Detalles = detalles
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);

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
                ErrorDispositivo = null,
                Patente = "XYZ789",
                VehiculoPresente = false,
                FechaEvento = fechaEvento,
                Detalles = null
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.CodigoDispositivo == "DISP002" &&
                       log.Patente == "XYZ789" &&
                       log.VehiculoPresente == false &&
                       log.ErrorDispositivo == "No se obtuvo información de los dispositivos"
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
                ErrorDispositivo = "Error de comunicación con cámara",
                Patente = null,
                VehiculoPresente = false,
                FechaEvento = fechaEvento,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando);

            // Assert
            Assert.That(resultado, Is.Not.Null);

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.ErrorDispositivo == "No se obtuvo información de los dispositivos" &&
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

            repositorioMock.Verify(r => r.Agregar(It.Is<LogIdentificacionVehicular>(
                log => log.ErrorDispositivo == "No se obtuvo información de los dispositivos"
            )), Times.Once());
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicularDetalle>()), Times.Never());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());
        }

        [Test]
        public void TestProcesamientoExitoso()
        {
            // Arrange
            var puesto = new PuestoDeTrabajo
            {
                Id = 100,
                NombrePuesto = "Puesto 1",
                CodigoConfigIdentificacionVehicular = "DISP001",
                Centro = new Centro { Id = 1 },
                VideoCamaras = new List<VideoCamara>()
            };

            repositorioMock.Setup(r => r.ObtenerProyeccion<PuestoDeTrabajo, int?>(
                    It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>(),
                    It.IsAny<Expression<Func<PuestoDeTrabajo, int?>>>()
                )).Returns((int?)100);
            repositorioMock.Setup(r => r.ObtenerProyeccion<Recorrido, int>(
                    It.IsAny<Expression<Func<Recorrido, bool>>>(),
                    It.IsAny<Expression<Func<Recorrido, int>>>()
                )).Returns(200);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<object>()))
                .Returns(puesto);

            servicioComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                .Returns(new Resultado());
            servicioRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(false);
            servicioRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = "TARJ001",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                ErrorDispositivo = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.LecturaPuestoDeTrabajo, Is.Not.Null);
            Assert.That(resultado.LecturaPuestoDeTrabajo.PuestoDeTrabajoId, Is.EqualTo(100));
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
            repositorioMock.Verify(r => r.GuardarCambios(), Times.AtLeastOnce());
        }

        [Test]
        public void TestPuestoNoEncontrado()
        {
            // Arrange
            repositorioMock.Setup(r => r.ObtenerProyeccion<PuestoDeTrabajo, int?>(
                    It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>(),
                    It.IsAny<Expression<Func<PuestoDeTrabajo, int?>>>()
                )).Returns((int?)null);

            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP_INEXISTENTE",
                Tarjeta = "TARJ001",
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                ErrorDispositivo = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.LecturaPuestoDeTrabajo, Is.Null);
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
                CodigoConfigIdentificacionVehicular = "DISP001",
                Centro = new Centro { Id = 1 },
                VideoCamaras = new List<VideoCamara>()
            };

            repositorioMock.Setup(r => r.ObtenerProyeccion<PuestoDeTrabajo, int?>(
                    It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>(),
                    It.IsAny<Expression<Func<PuestoDeTrabajo, int?>>>()
                )).Returns((int?)100);
            repositorioMock.Setup(r => r.ObtenerProyeccion<Recorrido, int>(
                    It.IsAny<Expression<Func<Recorrido, bool>>>(),
                    It.IsAny<Expression<Func<Recorrido, int>>>()
                )).Returns(0);
            repositorioMock.Setup(r => r.Obtener<PuestoDeTrabajo>(It.IsAny<object>()))
                .Returns(puesto);

            servicioComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                .Returns(new Resultado());

            var comando = new CrearLogIdentificacionVehicular
            {
                CodigoDispositivo = "DISP001",
                Tarjeta = null,
                Patente = "ABC123",
                VehiculoPresente = true,
                FechaEvento = DateTime.Now,
                ErrorDispositivo = null,
                Detalles = new List<ResultadoIntentoALPR>()
            };

            // Act
            var resultado = target.Ejecutar(comando) as ResultadoProcesarIdentificacionVehicular;

            // Assert
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.LecturaPuestoDeTrabajo, Is.Not.Null);
            Assert.That(resultado.LecturaPuestoDeTrabajo.PuestoDeTrabajoId, Is.EqualTo(100));
            repositorioMock.Verify(r => r.Agregar(It.IsAny<LogIdentificacionVehicular>()), Times.Once());
        }
    }
}
