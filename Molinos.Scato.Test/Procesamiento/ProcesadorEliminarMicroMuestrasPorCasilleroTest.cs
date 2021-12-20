using System;
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
    public class ProcesadorEliminarMicroMuestrasPorCasilleroTest
    {
        private ProcesadorEliminarMicroMuestrasPorCasillero target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private MicroMuestrasPorCasilleroDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarMicroMuestrasPorCasillero(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MicroMuestrasPorCasilleroDto
            {
                Id = 1,
                MuestraId = 1,
                CasilleroId = 1,
                Fecha = DateTime.Now
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarMicroMuestrasPorCasillero() { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<MicroMuestrasPorCasillero>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
