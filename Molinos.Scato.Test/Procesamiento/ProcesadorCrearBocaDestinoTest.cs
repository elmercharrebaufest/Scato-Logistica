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
    public class ProcesadorCrearBocaDestinoTest
    {
        private ProcesadorCrearBocaDestino target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private BocaDestinoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearBocaDestino(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new BocaDestinoDto
                {
                    Id = 1,
                    NombreBocaDeDestino = "a"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearBocaDestino {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<BocaDestino>(o => o.NombreBocaDeDestino == tipoDto.NombreBocaDeDestino)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}