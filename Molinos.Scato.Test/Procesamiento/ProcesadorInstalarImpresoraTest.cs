using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorInstalarImpresoraTest
    {
        private ProcesadorInstalarImpresora target;
        private Mock<IRepositorio> repositorioMock;
        private ImpresoraDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            var conversor = FactoryConversor.ConversorAutoMapper;

            target = new ProcesadorInstalarImpresora(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ImpresoraDto
                {
                    Id = 1,
                    Descripcion = "b",
                };
        }

        [Test]
        public void TestInstalarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                    .Returns(new Centro());

            var comando = new InstalarImpresora {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}
