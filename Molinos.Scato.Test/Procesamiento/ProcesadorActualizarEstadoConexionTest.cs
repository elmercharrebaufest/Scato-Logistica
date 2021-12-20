using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
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
    public class ProcesadorActualizarEstadoConexionTest
    {
        private ProcesadorActualizarEstadoConexion target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            log = new NullLogger();
            target = new ProcesadorActualizarEstadoConexion(repositorioMock.Object, conversorMock.Object, log);
        }

        [Test]
        public void TestCodigoDeDespositivoDesconocido()
        {
            repositorioMock.Setup(s => s.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<PuestoDeTrabajo>() { new PuestoDeTrabajo() { Centro = new Centro(), Estados = new List<EstadoConexion>() } });

            var resultado = target.Ejecutar(new ActualizarEstadoConexion { Dto = new EstadoConexionDto() }) as ResultadoActualizarEstadoConexion;

            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.EstadoConexionDto.First().PuestoDeTrabajoId, Is.EqualTo(0));
        }

        [Test]
        public void TestSinEstadoPrevio()
        {
            repositorioMock.Setup(s => s.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<PuestoDeTrabajo>() { new PuestoDeTrabajo { Centro = new Centro { Id = 1 }, Estados = new Collection<EstadoConexion>() } });
            repositorioMock.Setup(s => s.Obtener<EstadoConexion>(It.IsAny<Expression<Func<EstadoConexion, bool>>>())).Returns((EstadoConexion)null);

            var resultado = target.Ejecutar(new ActualizarEstadoConexion { Dto = new EstadoConexionDto() { Estado = true } }) as ResultadoActualizarEstadoConexion;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<EstadoConexion>()), Times.Once());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.EstadoConexionDto.First().Estado, Is.EqualTo(true));
        }

        [Test]
        public void TestConEstadoPrevio()
        {
            repositorioMock.Setup(s => s.Listar<PuestoDeTrabajo>(It.IsAny<Expression<Func<PuestoDeTrabajo, bool>>>())).Returns(new List<PuestoDeTrabajo>() { new PuestoDeTrabajo { Centro = new Centro { Id = 1 }, Estados = new Collection<EstadoConexion> { new EstadoConexion() } } });
            repositorioMock.Setup(s => s.Obtener<EstadoConexion>(It.IsAny<Expression<Func<EstadoConexion, bool>>>())).Returns(new EstadoConexion());

            var resultado = target.Ejecutar(new ActualizarEstadoConexion { Dto = new EstadoConexionDto() { Estado = true } }) as ResultadoActualizarEstadoConexion;

            repositorioMock.Verify(v => v.Agregar(It.IsAny<EstadoConexion>()), Times.Never());
            repositorioMock.Verify(v => v.GuardarCambios(), Times.Once());
            Assert.That(resultado.EstadoConexionDto.First().Estado, Is.EqualTo(true));
        }
    }
}