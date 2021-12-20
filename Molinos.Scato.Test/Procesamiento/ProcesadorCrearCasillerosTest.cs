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
    public class ProcesadorCrearCasillerosTest
    {
        private ProcesadorCrearCasilleros target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private CasilleroDto tipoDto;
        private CasilleroDto tipoDto2;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCasilleros(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new CasilleroDto
            {
                Id = 1,
                CentroId = 1,
                Capacidad = 50,
                Numero = "1111-111111"
            };

            tipoDto2 = new CasilleroDto
            {
                Id = 2,
                CentroId = 2,
                Capacidad = 25,
                Numero = "2222-222222"
            };
        }

        [Test]
        public void TestEjecutarConUnCasillero()
        {
            var comando = new CrearCasilleros { Dto = new List<CasilleroDto>{tipoDto} };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Casillero>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarConMasDeUnCasillero()
        {
            var comando = new CrearCasilleros { Dto = new List<CasilleroDto> { tipoDto, tipoDto2 } };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Casillero>()), Times.Exactly(2));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}