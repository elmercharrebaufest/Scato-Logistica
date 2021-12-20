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
    public class ProcesadorModificarVariedadPorVinedoTest
    {
        private ProcesadorModificarVariedadPorVinedo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private VariedadPorVinedoDto tipoDto;
        private VariedadPorVinedo tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarVariedadPorVinedo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new VariedadPorVinedoDto
                {
                    Id = 1,
                    Cosecha = "2010"
                };
            tipo = new VariedadPorVinedo
                {
                    Id = 1,
                    Cosecha = "2010"
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(
                x => x.Existe<VariedadPorVinedo>(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>())).Returns(false);
            repositorioMock.Setup(
                x => x.Existe<Variedad>(It.IsAny<Expression<Func<Variedad, bool>>>())).Returns(true);

            repositorioMock.Setup(s => s.Obtener<VariedadPorVinedo>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarVariedadPorVinedo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<VariedadPorVinedo>
                {
                    new VariedadPorVinedo
                        {
                            Id = 1,
                            Cosecha = "2010"
                        },
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>()))
                           .Returns<Expression<Func<VariedadPorVinedo, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>()))
                           .Returns<Expression<Func<VariedadPorVinedo, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<VariedadPorVinedo>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(
                x => x.Existe<Variedad>(It.IsAny<Expression<Func<Variedad, bool>>>())).Returns(true);

            var comando = new ModificarVariedadPorVinedo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}