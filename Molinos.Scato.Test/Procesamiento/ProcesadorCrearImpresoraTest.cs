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
    public class ProcesadorCrearImpresoraTest
    {
        private ProcesadorCrearImpresora target;
        private Mock<IRepositorio> repositorioMock;
        private ImpresoraDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            var conversor = FactoryConversor.ConversorAutoMapper;

            target = new ProcesadorCrearImpresora(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ImpresoraDto
                {
                    Id = 1,
                    Descripcion = "b",
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                    .Returns(new Centro());

            var comando = new CrearImpresora {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Impresora>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
