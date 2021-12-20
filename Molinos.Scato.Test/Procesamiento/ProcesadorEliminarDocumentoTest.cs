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
    public class ProcesadorEliminarDocumentoTest
    {
        private ProcesadorEliminarDocumento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ImpresionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarDocumento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ImpresionDto
            {
                Id = 1
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            repositorioMock.Setup(x => x.Obtener<Impresion>(It.IsAny<int>())).Returns(new ImpAsignacionDeRuta());
            var comando = new EliminarDocumento {Id = tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
