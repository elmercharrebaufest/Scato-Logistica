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
    public class ProcesadorEliminarControlDeTiempoTest
    {
        private ProcesadorEliminarControlDeTiempo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ControlDeTiempoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarControlDeTiempo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ControlDeTiempoDto
            {
                Id = 1,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarControlDeTiempo() {Id = (int)tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<ControlDeTiempo>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
