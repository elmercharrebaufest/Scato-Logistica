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
    public class ProcesadorImprimirEtiquetaAuditoriaTest
    {
        private ProcesadorImprimirEtiquetaAuditoria target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IFirmaProvider> firmaMock;
        private Mock<IServicioImpresorFactory> servicioImpresion;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            firmaMock = new Mock<IFirmaProvider>();
            servicioImpresion = new Mock<IServicioImpresorFactory>();

            target = new ProcesadorImprimirEtiquetaAuditoria(repositorioMock.Object, conversorMock.Object, new NullLogger(), firmaMock.Object, servicioImpresion.Object);
        }

        [Test]
        public void TestEjecutar()
        {

        }
    }
}
