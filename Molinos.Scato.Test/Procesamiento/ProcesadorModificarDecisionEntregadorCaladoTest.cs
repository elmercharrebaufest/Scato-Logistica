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
    public class ProcesadorModificarDecisionEntregadorCaladoTest
    {
        private ProcesadorModificarDecisionEntregadorCalado target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Calado tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarDecisionEntregadorCalado(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipo = new Calado
            {
                EstaAutorizadoPorEntregador = false
            };
        }

        [Test]
        public void TestModificarEntidad()
        {

            repositorioMock.Setup(s => s.Obtener<Calado>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarDecisionEntregadorCalado { CaladoId = 1,Decision = true};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(tipo.EstaAutorizadoPorEntregador, true);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
