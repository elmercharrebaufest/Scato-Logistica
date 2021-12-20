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
    public class ProcesadorModificarBocaDestinoTest
    {
        private ProcesadorModificarBocaDestino target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private BocaDestinoDto tipoDto;
        private BocaDestino tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarBocaDestino(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new BocaDestinoDto
                {
                    Id = 1,
                    NombreBocaDeDestino = "a"
                };
            tipo = new BocaDestino
                {
                    Id = 1,
                    NombreBocaDeDestino = "a"
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarBocaDestino {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<BocaDestino>
                {
                    new BocaDestino
                        {
                            Id = 1,
                            NombreBocaDeDestino = "a"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<BocaDestino, bool>>>()))
                           .Returns<Expression<Func<BocaDestino, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<BocaDestino>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarBocaDestino { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}