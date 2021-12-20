using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorLibroMovimientosExistenciaDeGranosExcelTest
    {
        private ProcesadorImprimirDocumento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IFirmaProvider> firmaProvider;
        private Mock<IServicioImpresion> servicioImpresion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            firmaProvider = new Mock<IFirmaProvider>();
            servicioImpresion = new Mock<IServicioImpresion>();

            target = new ProcesadorImprimirDocumento(repositorioMock.Object, conversorMock.Object, new NullLogger(), firmaProvider.Object, servicioImpresion.Object);
        }

        [Test]
        public void TestEjecutar()
        {

        }
    }
}
