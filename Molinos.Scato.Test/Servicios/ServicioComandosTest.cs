using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;
using Ninject;

namespace Molinos.Scato.Test.Servicios
{
    [TestFixture]
    public class ServicioComandosTest
    {
        [Test]
        public void TestInicializacion()
        {
            var kernelMock = new Mock<IKernel>();
            Assert.That(() => new ServicioComandos(kernelMock.Object, new NullLogger()), Throws.Nothing);
        }

    }
}
