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
   public class ProcesadorEliminarTaraRomaneoTest
    {
         private ProcesadorEliminarTaraRomaneo target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TaraRomaneoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTaraRomaneo(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TaraRomaneoDto
            {
                Id = 1,
                Descripcion = "tara 1",
                CentroId = 1,
                Peso = 500,
                Importacion = true,
                CargaPesoManual = false
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarTaraRomaneo() { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<TaraRomaneo>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
