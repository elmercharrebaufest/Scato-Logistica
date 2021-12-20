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
    public class ProcesadorEliminarTipoComercialTest
    {
        private ProcesadorEliminarTipoComercial target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TipoComercialDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTipoComercial(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TipoComercialDto
            {
                Id = 1,
                Descripcion = "Modalidad nro 1",
                PesoEsperado = 3,
                Sentido = "E",
                ToleranciaDifPesoE = 4,
                UsaBinPallet = true,
                ValidaPatente = false
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarTipoComercial {Id = tipoDto.Id.Value};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<TipoComercial>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
