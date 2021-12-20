using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class ProcesadorEliminarCasillerosTest
    {
        private ProcesadorEliminarCasilleros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarCasilleros(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEjecutarConUnCasillero()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>()))
                           .Returns(new List<MicroMuestrasPorCasillero>());

            var comando = new EliminarCasilleros { Id = new List<int> { 1 } };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Casillero>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConMasDeUnCasillero()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>()))
                           .Returns(new List<MicroMuestrasPorCasillero>());

            var comando = new EliminarCasilleros { Id = new List<int> { 1, 2 } };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Casillero>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConErrorPorExistenciaDeMicroMuestras()
        {
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>()))
                .Returns(new List<MicroMuestrasPorCasillero>
                     {
                         new MicroMuestrasPorCasillero { Id = 1, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } }
                     });

            var comando = new EliminarCasilleros { Id = new List<int> { 1 } };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Casillero>(It.IsAny<int>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}