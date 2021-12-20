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
    public class ProcesadorEliminarMotivoTest
    {
        private ProcesadorEliminarMotivo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private MotivoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarMotivo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new MotivoDto
                {
                    Id = 1,
                    Descripcion = "motivo",
                    DescripcionCorta = "mot"
                };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarMotivo {Id = tipoDto.Id.Value};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Motivo>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
