using System.Reflection;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorImprimirCertificadoDeCartaPorteTest
    {
        private ProcesadorImprimirCertificadoDeCartaPorte target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IServicioImpresorFactory> servicioImpresorFactoryMock;
        private ImprimirCertificadoDeCartaPorte comando;    

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioImpresorFactoryMock = new Mock<IServicioImpresorFactory>();

            target = new ProcesadorImprimirCertificadoDeCartaPorte(repositorioMock.Object, conversorMock.Object, new NullLogger(), servicioImpresorFactoryMock.Object);
        }

        //[Test]
        public void TestEjecutar()
        {

        }

        [Test]
        public void DebeImprimir_CuandoConfiguracionEnMaterialPorWorkflowNoDebeSerConsiderada_DebeRetornarTrue()
        {
            // Arrange

            // Act: invocamos el protected override DebeImprimir vía reflection
            var method = typeof(ProcesadorImprimirCertificadoDeCartaPorte).GetMethod("DebeImprimir", BindingFlags.NonPublic | BindingFlags.Instance);
            bool resultado = (bool)method.Invoke(target, new object[] { comando });

            // Assert
            Assert.IsTrue(resultado);
        }
    }
}
