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
    public class ProcesadorModificarCentroTest
    {
        private ProcesadorModificarCentro target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CentroDto tipoDto;
        private Centro tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarCentro(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new CentroDto
            {
                Id = 1,
                Descripcion = "Centro 1"
            };
            tipo = new Centro
            {
                Id = 1,
                Descripcion = "Almacen Dto 1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarCentro { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<CentroDto>(), It.IsAny<Centro>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
