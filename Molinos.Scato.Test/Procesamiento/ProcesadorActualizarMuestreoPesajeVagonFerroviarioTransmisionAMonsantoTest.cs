using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsantoTest
    {
        private ProcesadorActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MuestreoPesajeVagonFerroviarioTransmisionAMonsanto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MuestreoPesajeVagonFerroviarioTransmisionAMonsanto()
            {
                Id = 1,
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new ActualizarMuestreoPesajeVagonFerroviarioTransmisionAMonsanto() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<MuestreoPesajeVagonFerroviarioTransmisionAMonsanto>(o => o.Id == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
