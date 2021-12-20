using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ExisteExcepcionAlControlTest
    {
        private Scato.Actividades.Internas.ExisteExcepcionAlControl target;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.ExisteExcepcionAlControl();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestExisteExcepcionAlControl()
        {
            srv.Setup(x => x.BuscarExcepcionAlControl(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            host.InArguments.CentroId = 1;
            host.InArguments.TransportistaId = 2;
            host.InArguments.MaterialId = 2;
            host.InArguments.CentroDestinoId = 2;
            host.InArguments.ClienteDestinoId = 1;

            var resultado = host.TestActivity();
            var result = resultado.First(f => f.Key == "Result").Value;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void TestExisteExcepcionAlControlError()
        {
            srv.Setup(x => x.BuscarExcepcionAlControl(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            host.InArguments.CentroId = 1;
            host.InArguments.TransportistaId = 2;
            host.InArguments.MaterialId = 2;
            host.InArguments.CentroDestinoId = 2;
            host.InArguments.ClienteDestinoId = 1;

            var resultado = host.TestActivity();
            var result = resultado.First(f => f.Key == "Result").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(false));
        }
    }
}
