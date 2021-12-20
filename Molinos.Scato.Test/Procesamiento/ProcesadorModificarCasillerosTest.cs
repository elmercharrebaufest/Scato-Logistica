using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class ProcesadorModificarCasillerosTest
    {
        private ProcesadorModificarCasilleros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarCasilleros(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEjecutarConUnCasillero()
        {
            var dto = new CasilleroDto
            {
                Id = 1,
                CentroId = 1,
                Capacidad = 30,
                Numero = "1111-111111"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>())).Returns(new List<MicroMuestrasPorCasillero>
                {
                    new MicroMuestrasPorCasillero { Id = 1, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } },
                    new MicroMuestrasPorCasillero { Id = 2, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } }
                });

            var comando = new ModificarCasilleros { Dto = new List<CasilleroDto>{dto}, Capacidad = 2 };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConMasDeUnCasillero()
        {
            var dto = new CasilleroDto
            {
                Id = 1,
                CentroId = 1,
                Capacidad = 30,
                Numero = "1111-111111"
            };

            var dto2 = new CasilleroDto
            {
                Id = 2,
                CentroId = 2,
                Capacidad = 5,
                Numero = "2222-222222"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>())).Returns(new List<MicroMuestrasPorCasillero>
                {
                    new MicroMuestrasPorCasillero { Id = 1, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } },
                    new MicroMuestrasPorCasillero { Id = 2, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } }
                });

            var comando = new ModificarCasilleros { Dto = new List<CasilleroDto> { dto, dto2 }, Capacidad = 3 };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConErrorPorCapacidad()
        {
            var dto = new CasilleroDto
            {
                Id = 1,
                CentroId = 1,
                Capacidad = 30,
                Numero = "1111-111111"
            };

            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<MicroMuestrasPorCasillero, bool>>>())).Returns(new List<MicroMuestrasPorCasillero>
                {
                    new MicroMuestrasPorCasillero { Id = 1, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } },
                    new MicroMuestrasPorCasillero { Id = 2, Muestra = new MuestraEnvioACamara{Id =1}, Casillero = new Casillero { Id = 1 } }
                });

            var comando = new ModificarCasilleros { Dto = new List<CasilleroDto> { dto }, Capacidad = 1};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}