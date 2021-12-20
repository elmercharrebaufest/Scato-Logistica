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
    public class ObtenerToleranciaOrigenTest
    {
        private ObtenerToleranciaOrigen target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new ObtenerToleranciaOrigen();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestObtenerToleranciaOrigen()
        {
            srvRepositorio.Setup(s => s.ObtenerToleranciaOrigen(It.IsAny<Guid>())).Returns(30000);
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var peso = resultado.First(f => f.Key == "Tolerancia").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(peso, Is.EqualTo(30000));

        }
    }
}
