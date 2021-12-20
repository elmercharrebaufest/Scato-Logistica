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
    public class ProcesadorModificarImpresoraTest
    {
        private ProcesadorModificarImpresora target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ImpresoraDto tipoDto;
        private Impresora tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarImpresora(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ImpresoraDto
                {
                    Id = 1,
                    Descripcion = "a",
                };
            tipo = new Impresora
                {
                    Id = 1,
                    Descripcion = "b",
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>()))
                    .Returns(new Centro());
            repositorioMock.Setup(s => s.Obtener<Impresora>(It.IsAny<int>())).Returns(tipo);
            

            var comando = new ModificarImpresora {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
        
    }
}
