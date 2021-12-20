using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorActualizarIngresosEgresosFazonesTransmisionASapTest
    {
        private ProcesadorActualizarIngresosEgresosFazonesTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private Dominio.Entidades.IngresosEgresosFazonesTransmisionASap tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarIngresosEgresosFazonesTransmisionASap(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new Molinos.Scato.Dominio.Entidades.IngresosEgresosFazonesTransmisionASap
                {
                     Id = 1
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
         
            var comando = new ActualizarIngresosEgresosFazonesTransmisionASap {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
