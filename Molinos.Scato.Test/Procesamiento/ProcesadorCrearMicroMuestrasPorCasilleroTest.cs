using System;
using System.Linq;
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
    public class ProcesadorCrearMicroMuestrasPorCasilleroTest
    {
        private ProcesadorCrearMicroMuestrasPorCasillero target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private MicroMuestrasPorCasilleroDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearMicroMuestrasPorCasillero(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MicroMuestrasPorCasilleroDto
            {
                MuestraId = 1,
                CasilleroId = 1,
                Fecha = DateTime.Now
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<MuestraEnvioACamara>(It.IsAny<int>())).Returns(new MuestraEnvioACamara{Id = 1, GeneroMicroMuestras = false});

            var comando = new CrearMicroMuestrasPorCasillero { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<MicroMuestrasPorCasillero>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }


        [Test]
        public void TestCrearEntidadInvalidoPorExistenciaDeGeneracion()
        {

            repositorioMock.Setup(s => s.Obtener<MuestraEnvioACamara>(It.IsAny<int>())).Returns(new MuestraEnvioACamara { Id = 1, GeneroMicroMuestras = true });

            var comando = new CrearMicroMuestrasPorCasillero { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<MicroMuestrasPorCasillero>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("ExisteGeneracion"));
        }
    }
}
