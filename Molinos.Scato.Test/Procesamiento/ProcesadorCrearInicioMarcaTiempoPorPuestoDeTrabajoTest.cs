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
    public class ProcesadorCrearInicioMarcaTiempoPorPuestoDeTrabajoTest
    {
        private ProcesadorCrearInicioMarcaTiempoPorPuestoDeTrabajo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorCrearInicioMarcaTiempoPorPuestoDeTrabajo(
                repositorioMock.Object, conversorMock.Object, log);

            recorrido = new Recorrido
            {
                Id = 1,
                TarjetaDeAcceso = "123456",
                Patente = "ABC123",
                InstanciaWorkflow = new Guid("25892e17-80f6-415f-9c65-7395632f0223"),
                NumeroDocumentoIngreso = "DOC001",
                Terminado = false
            };
        }

        [Test]
        public void TestCrear_PorTarjeta_InsertaRegistroConFechaInicio()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(recorrido);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(false);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDeTarjeta   = "123456",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.Is<MarcaTiempoPorPuestoDeTrabajo>(
                e => e.PuestoDeTrabajoId == 1
                  && e.NumeroDocumento == recorrido.NumeroDocumentoIngreso
                  && e.Centro_Id == 5
                  && e.FechaInicio.HasValue)),
                Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear_PorPatente_InsertaRegistroConFechaInicio()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(recorrido);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(false);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                Patente           = "ABC123",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.Is<MarcaTiempoPorPuestoDeTrabajo>(
                e => e.PuestoDeTrabajoId == 1 && e.FechaInicio.HasValue)),
                Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear_SinRecorrido_NoInserta()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns((Recorrido)null);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDeTarjeta   = "999999",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestCrear_RegistroDuplicado_NoInserta()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(recorrido);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(true);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDeTarjeta   = "123456",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestCrear_HappyPath_InsertaRegistroConFechaInicio()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(recorrido);
            repositorioMock
                .Setup(s => s.Existe(It.IsAny<Expression<Func<MarcaTiempoPorPuestoDeTrabajo, bool>>>()))
                .Returns(false);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDeTarjeta   = "123456",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.Is<MarcaTiempoPorPuestoDeTrabajo>(
                e => e.PuestoDeTrabajoId == 1
                  && e.NumeroDocumento == recorrido.NumeroDocumentoIngreso
                  && e.Centro_Id == 5
                  && e.FechaInicio.HasValue)),
                Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrear_SinRecorrido_NoInsertaNiBloquea()
        {
            repositorioMock
                .Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns((Recorrido)null);

            var comando = new CrearInicioMarcaTiempoPorPuestoDeTrabajo
            {
                NumeroDeTarjeta   = "999999",
                PuestoDeTrabajoId = 1,
                CentroId          = 5
            };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.Agregar(It.IsAny<MarcaTiempoPorPuestoDeTrabajo>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}
