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
    public class ProcesadorEliminarBocaDestinoTest
    {
        private ProcesadorEliminarBocaDestino target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private BocaDestinoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarBocaDestino(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new BocaDestinoDto
            {
                Id = 1,
                NombreBocaDeDestino = "a"
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarBocaDestino {Id = tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<BocaDestino>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
