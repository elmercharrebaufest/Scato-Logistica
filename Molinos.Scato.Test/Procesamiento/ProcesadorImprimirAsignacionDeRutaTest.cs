using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorImprimirAsignacionDeRutaTest
    {
        private ProcesadorImprimirAsignacionDeRuta target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IServicioImpresorFactory> servicioImpresion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioImpresion = new Mock<IServicioImpresorFactory>();

            target = new ProcesadorImprimirAsignacionDeRuta(repositorioMock.Object, conversorMock.Object, new NullLogger(), servicioImpresion.Object);
        }

        [Test]
        public void TestEjecutar()
        {

        }
    }
}
