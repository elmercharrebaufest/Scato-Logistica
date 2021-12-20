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
    public class ProcesadorEliminarCaracteristicaDeCalidadTest
    {
        private ProcesadorEliminarCaracteristicaDeCalidad target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CaracteristicaDeCalidadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarCaracteristicaDeCalidad(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new CaracteristicaDeCalidadDto
            {
                Id = 1,
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var caracteristica = new CaracteristicaDeCalidad()
                {
                    Id = 1,
                    EsHumedad = false
                };
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>()))
                    .Returns(caracteristica);
            var comando = new EliminarCaracteristicaDeCalidad { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover(It.Is<CaracteristicaDeCalidad>(c => c.Id == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
