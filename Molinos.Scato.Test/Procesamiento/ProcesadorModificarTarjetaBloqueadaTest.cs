using System;
using System.Collections.Generic;
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
    public class ProcesadorModificarTarjetaBloqueadaTest
    {
        private ProcesadorModificarTarjetaBloqueada target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TarjetaBloqueadaDto tipoDto;
        private TarjetaBloqueada tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarTarjetaBloqueada(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TarjetaBloqueadaDto
            {
                Id = 1,
                Numero = "11111"

            };
            tipo = new TarjetaBloqueada
            {
                Id = 1,
                Numero = "11111"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TarjetaBloqueada>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTarjetaBloqueada { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorExistencia()
        {
            var tiposExistentes = new List<TarjetaBloqueada>
                {
                    new TarjetaBloqueada {Id = 5, Numero = "11111",Centro = new Centro()},
                    new TarjetaBloqueada {Id = 6, Numero = "22222",Centro = new Centro() }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TarjetaBloqueada, bool>>>()))
                    .Returns<Expression<Func<TarjetaBloqueada, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Numero = "11111";
            var comando = new ModificarTarjetaBloqueada { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Numero"));
        }
    }
}
