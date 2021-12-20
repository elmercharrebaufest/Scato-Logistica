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
    public class ProcesadorEliminarConversionGrupoTest
    {
        private ProcesadorEliminarConversionGrupo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ConversionGrupoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarConversionGrupo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ConversionGrupoDto
            {
                Id = 1,
                CamaraId = 1
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarConversionGrupo() {Id = (int)tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<ConversionGrupo>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
