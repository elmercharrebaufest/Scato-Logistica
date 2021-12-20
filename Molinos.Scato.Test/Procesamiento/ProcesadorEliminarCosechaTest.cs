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
    public class ProcesadorEliminarCosechaTest
    {
        private ProcesadorEliminarCosecha target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CosechaDto cosechaDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarCosecha(repositorioMock.Object, conversorMock.Object, new NullLogger());
            cosechaDto = new CosechaDto
            {
                Id = 1,
                Descripcion = "11-12",
                EpaPesoDescontado = true
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarCosecha { Id = cosechaDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Cosecha>(It.Is<object>(o => (int)o == cosechaDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
