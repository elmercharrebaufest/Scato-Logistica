using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class LeerPesoMaximoTest
    {
        private LeerPesoMaximo target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new LeerPesoMaximo();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestLeerPesoMaximo()
        {
            srvRepositorio.Setup(s => s.LeerPesoMaximo(It.IsAny<Guid>(), It.IsAny<TipoDeWorkflow>())).Returns(30000);
            host.InArguments.TipoPesoMaximo = TipoDeWorkflow.Ingreso;
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var peso = resultado.First(f => f.Key == "PesoMaximo").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(peso, Is.EqualTo(30000));

        }
    }
}
