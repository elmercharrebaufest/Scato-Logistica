using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
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

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorConsultarLecturaPorIdentificacionVehicularTest
    {
        private ProcesadorConsultarLecturaPorIdentificacionVehicular target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private NullLogger log;

        private static readonly string FirmwareGaritaIngreso =
            "Molinos.Scato.Web.Firmware.FirmwareGaritaIngreso, Molinos.Scato.Web";

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            log = new NullLogger();
            target = new ProcesadorConsultarLecturaPorIdentificacionVehicular(
                repositorioMock.Object, conversor, log);
        }

        // --- Ciclo 1: Propiedades del comando ---

        [Test]
        public void ConsultarLecturaPorIdentificacionVehicular_TienePropiedadesEsperadas()
        {
            var comando = new ConsultarLecturaPorIdentificacionVehicular
            {
                CodigoDispositivo = "CFG-IDVEH-01",
                Tarjeta = "123456"
            };

            Assert.That(comando.CodigoDispositivo, Is.EqualTo("CFG-IDVEH-01"));
            Assert.That(comando.Tarjeta, Is.EqualTo("123456"));
        }

        // --- Ciclo 2: Estructura del resultado ---

        [Test]
        public void ResultadoConsultarLecturaPorIdentificacionVehicular_TieneEstructuraEsperada()
        {
            var resultado = new ResultadoConsultarLecturaPorIdentificacionVehicular
            {
                Lectura = new LecturaPuestoDeTrabajoDto { PuestoDeTrabajoId = 5 },
                Firmware = FirmwareGaritaIngreso
            };

            Assert.That(resultado.Lectura.PuestoDeTrabajoId, Is.EqualTo(5));
            Assert.That(resultado.Firmware, Is.Not.Null);
        }

        // --- Ciclo 3: Puesto no encontrado ---

        [Test]
        public void Ejecutar_PuestoNoEncontrado_RetornaError()
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                .Returns((PuestoDeTrabajo)null);

            var resultado = EjecutarConTarjeta("INEXISTENTE", "123456");

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Lectura, Is.Null);
        }

        // --- Ciclo 4: Puesto sin firmware ---

        [Test]
        public void Ejecutar_PuestoSinFirmware_RetornaError()
        {
            ConfigurarPuesto(CrearPuesto(firmware: null));

            var resultado = EjecutarConTarjeta("CFG-01", "123456");

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Firmware, Is.Null);
        }

        // --- Ciclo 5: Puesto con firmware y tarjeta válida ---

        [Test]
        public void Ejecutar_PuestoConFirmware_ConstruyeLecturaConDatosPuesto()
        {
            var puesto = CrearPuesto(firmware: FirmwareGaritaIngreso);
            ConfigurarPuesto(puesto);
            ConfigurarTarjetaValida("123456", puesto.Centro.Id);

            var resultado = EjecutarConTarjeta("CFG-01", "123456");

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.Lectura, Is.Not.Null);
            Assert.That(resultado.Lectura.PuestoDeTrabajoId, Is.EqualTo(puesto.Id));
            Assert.That(resultado.Lectura.NumeroDeTarjeta, Is.EqualTo("123456"));
            Assert.That(resultado.Lectura.PuestoDeTrabajoPidePantente, Is.EqualTo(puesto.PidePatente));
            Assert.That(resultado.Lectura.CentroId, Is.EqualTo(puesto.Centro.Id));
            Assert.That(resultado.Lectura.Firmware, Is.EqualTo(puesto.Firmware));
            Assert.That(resultado.Lectura.TarjetaValida, Is.True);
            Assert.That(resultado.Firmware, Is.EqualTo(puesto.Firmware));
        }

        // --- Ciclo 6: Tarjeta inválida (fuera de rango) ---

        [Test]
        public void Ejecutar_TarjetaFueraDeRango_RetornaLecturaConTarjetaInvalida()
        {
            var puesto = CrearPuesto(firmware: FirmwareGaritaIngreso);
            ConfigurarPuesto(puesto);

            repositorioMock
                .Setup(r => r.Existe<TarjetaBloqueada>(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                .Returns(false);
            repositorioMock
                .Setup(r => r.Existe<TarjetaRango>(It.IsAny<Expression<Func<TarjetaRango, bool>>>()))
                .Returns(false);

            var resultado = EjecutarConTarjeta("CFG-01", "999999");

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.Lectura.TarjetaValida, Is.False);
            Assert.That(resultado.Lectura.MensajeError, Is.Not.Null.And.Not.Empty);
        }

        // --- Ciclo 7: Sin tarjeta (solo patente) ---

        [Test]
        public void Ejecutar_SinTarjeta_RetornaLecturaConTarjetaVaciaYSinError()
        {
            var puesto = CrearPuesto(firmware: FirmwareGaritaIngreso);
            ConfigurarPuesto(puesto);

            var resultado = EjecutarConTarjeta("CFG-01", null);

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.Lectura.TarjetaValida, Is.False);
            Assert.That(resultado.Lectura.NumeroDeTarjeta, Is.Null);
        }

        // --- Helpers ---

        private ResultadoConsultarLecturaPorIdentificacionVehicular EjecutarConTarjeta(string codigoDispositivo, string tarjeta)
        {
            return target.Ejecutar(new ConsultarLecturaPorIdentificacionVehicular
            {
                CodigoDispositivo = codigoDispositivo,
                Tarjeta = tarjeta
            }) as ResultadoConsultarLecturaPorIdentificacionVehicular;
        }

        private PuestoDeTrabajo CrearPuesto(string firmware)
        {
            return new PuestoDeTrabajo
            {
                Id = 10,
                NombrePuesto = "Garita Ingreso",
                CodigoConfigIdentificacionVehicular = "CFG-01",
                Firmware = firmware,
                PidePatente = true,
                Centro = new Centro { Id = 1 },
                VideoCamaras = new Collection<VideoCamara>(),
                Lecturas = new Collection<LecturaDeTarjeta>()
            };
        }

        private void ConfigurarPuesto(PuestoDeTrabajo puesto)
        {
            repositorioMock
                .Setup(r => r.ObtenerPrimero<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>()))
                .Returns(puesto);
            repositorioMock
                .Setup(r => r.ObtenerMenor<LecturaDeTarjeta, int>(
                    It.IsAny<Expression<Func<LecturaDeTarjeta, bool>>>(),
                    It.IsAny<Expression<Func<LecturaDeTarjeta, int>>>()))
                .Returns((LecturaDeTarjeta)null);
        }

        private void ConfigurarTarjetaValida(string tarjeta, int centroId)
        {
            repositorioMock
                .Setup(r => r.Existe<TarjetaBloqueada>(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                .Returns(false);
            repositorioMock
                .Setup(r => r.Existe<TarjetaRango>(It.IsAny<Expression<Func<TarjetaRango, bool>>>()))
                .Returns(true);
            repositorioMock
                .Setup(r => r.Existe<TarjetaSupervisor>(It.IsAny<Expression<Func<TarjetaSupervisor, bool>>>()))
                .Returns(false);
        }
    }
}
