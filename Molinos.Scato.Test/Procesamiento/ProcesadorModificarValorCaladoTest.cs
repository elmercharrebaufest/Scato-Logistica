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
    public class ProcesadorModificarValorCaladoTest
    {
        private ProcesadorModificarValorCalado target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private IList<CaladoPorCaracteristicaDto> tiposDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarValorCalado(repositorioMock.Object, conversor, new NullLogger());
            tiposDto = new List<CaladoPorCaracteristicaDto>
            {
                new CaladoPorCaracteristicaDto
                    {
                        Id = 1, ValorCalado = 10, CaracteristicaId = 1, Caracteristica = "Caracteristica 1"
                    }, 
                new CaladoPorCaracteristicaDto
                    {
                        Id = 2, ValorCalado = 5, CaracteristicaId = 2, Caracteristica = "Caracteristica 2"
                    }
            };
        }

        [Test]
        public void TestEjecutar()
        {
            repositorioMock.Setup(s => s.Obtener<CaladoPorCaracteristica>(It.IsAny<int>()))
                .Returns(new CaladoPorCaracteristica{Id = 1, ValorCalado = 20});

            var comando = new ModificarValorCalado { Dto = new AjusteDeCalidadDto() };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}