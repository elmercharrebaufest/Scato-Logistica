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
    public class ProcesadorEliminarTecnologiaTest
    {
        private ProcesadorEliminarTecnologia target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TecnologiaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTecnologia(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TecnologiaDto()
            {
                Id = 1,
                Codigo = "C1",
                Nombre = "Tecnologia Nro 1"
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarTecnologia() {Id = tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Tecnologia>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
