using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarProveedorExcluidoIntactaTest
    {
        private VerificarProveedorExcluidoIntacta target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarProveedorExcluidoIntacta();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarProveedorExcluidoIntacta()
        {
            srvRepositorio.Setup(s => s.EsProveedorExcluidoIntacta(It.IsAny<int>())).Returns(true);

            host.InArguments.ProveedorId = 6;
            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "EsProveedorExcluido").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(true));
        }

        [Test]
        public void TestVerificarProveedorExcluidoIntactaFalso()
        {
            srvRepositorio.Setup(s => s.EsProveedorExcluidoIntacta(It.IsAny<int>())).Returns(false);

            host.InArguments.ProveedorId = 6;
            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "EsProveedorExcluido").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(false));
        }
    }
}
