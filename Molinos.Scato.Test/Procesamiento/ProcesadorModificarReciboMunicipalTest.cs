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
    public class ProcesadorModificarReciboMunicipalTest
    {
        private ProcesadorModificarReciboMunicipal target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ReciboMunicipalDto tipoDto;
        private ReciboMunicipal tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarReciboMunicipal(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ReciboMunicipalDto
            {
                Id = 1,
                Monto = 50,
                Ordenanza = "Ordenanza",
                CentroId = 1
            };
            tipo = new ReciboMunicipal
            {
                Id = 1,
                Monto = 50,
                Ordenanza = "Ordenanza"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<ReciboMunicipal>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarReciboMunicipal { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<ReciboMunicipalDto>(), It.IsAny<ReciboMunicipal>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
