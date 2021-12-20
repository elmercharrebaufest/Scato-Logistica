using System;
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
    public class ProcesadorCrearVariedadPorVinedoTest
    {
        private ProcesadorCrearVariedadPorVinedo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private VariedadPorVinedoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearVariedadPorVinedo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new VariedadPorVinedoDto
                {
                    Id = 1,
                    Cosecha = "20-10"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(
                x => x.Existe<VariedadPorVinedo>(It.IsAny<Expression<Func<VariedadPorVinedo, bool>>>())).Returns(false);
            repositorioMock.Setup(
                x => x.Existe<Variedad>(It.IsAny<Expression<Func<Variedad, bool>>>())).Returns(true);

            var comando = new CrearVariedadPorVinedo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<VariedadPorVinedo>(o => o.Cosecha == comando.Dto.Cosecha)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}