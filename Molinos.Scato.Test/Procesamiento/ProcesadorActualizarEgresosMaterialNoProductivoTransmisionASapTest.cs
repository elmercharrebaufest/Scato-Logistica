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
    public class ProcesadorActualizarEgresosMaterialNoProductivoTransmisionASapTest
    {
        private ProcesadorActualizarEgresosMaterialNoProductivoTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private EgresosNoProductivosTransmisionASap tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarEgresosMaterialNoProductivoTransmisionASap(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new EgresosNoProductivosTransmisionASap
                {
                    Id = 1,
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
         
            var comando = new ActualizarEgresosMaterialNoProductivoTransmisionASap {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
