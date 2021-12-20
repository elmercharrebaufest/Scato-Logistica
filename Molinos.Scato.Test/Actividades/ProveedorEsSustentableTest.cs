using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ProveedorEsSustentableTest
    {
        private ProveedorEsSustentable target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new ProveedorEsSustentable();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestProveedorEsSustentable()
        {
            srvRepositorio.Setup(s => s.EsProveedorSustentable(It.IsAny<int>())).Returns(true);
            host.InArguments.ProveedorId = 1;

            var resultado = host.TestActivity();

            var peso = resultado.First(f => f.Key == "EsSustentable").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(peso, Is.EqualTo(true));

        }

        [Test]
        public void TestNoProveedorEsSustentable()
        {
            srvRepositorio.Setup(s => s.EsProveedorSustentable(It.IsAny<int>())).Returns(false);
            host.InArguments.ProveedorId = 1;

            var resultado = host.TestActivity();

            var peso = resultado.First(f => f.Key == "EsSustentable").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(peso, Is.EqualTo(false));

        }
    }
}
