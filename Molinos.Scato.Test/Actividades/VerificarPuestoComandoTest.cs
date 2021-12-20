using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarPuestoComandoTest
    {
        private VerificarPuestoComando target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarPuestoComando();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestAsignado()
        {
            host.InArguments.WorkflowId = new Guid();
            srvRepositorio.Setup(s => s.WorkflowEstaAsignado(It.IsAny<Guid>())).Returns(true);
            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "EstaAsignado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void TestNoAsignado()
        {
            host.InArguments.WorkflowId = new Guid();
            srvRepositorio.Setup(s => s.WorkflowEstaAsignado(It.IsAny<Guid>())).Returns(false);
            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "EstaAsignado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }

    }
}
