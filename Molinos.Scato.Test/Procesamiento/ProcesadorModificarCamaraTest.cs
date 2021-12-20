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
    public class ProcesadorModificarCamaraTest
    {
        private ProcesadorModificarCamara target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CamaraDto tipoDto;
        private Camara tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarCamara(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new CamaraDto
            {
                Id = 1,
                CodigoSAP = "asd"
            };
            tipo = new Camara
            {
                Id = 1,
                CodigoSAP = "asd"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {

            repositorioMock.Setup(s => s.Obtener<Camara>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarCamara { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<CamaraDto>(), It.IsAny<Camara>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
