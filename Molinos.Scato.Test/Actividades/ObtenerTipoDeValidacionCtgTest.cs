using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ObtenerTipoDeValidacionCtgTest
    {
        private ObtenerTipoDeValidacionCtg target;
        private Mock<IServicioRepositorio> servicio;
        private WorkflowInvokerTest host;

        [Test]
        public void TestValidacionAutomatica()
        {
            target = new ObtenerTipoDeValidacionCtg();
            servicio = new Mock<IServicioRepositorio>();
            servicio.Setup(s => s.ValidacionCtgEsAutomatica(1)).Returns(true);
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servicio.Object);
            host.InArguments.CentroId = 1;
            host.TestActivity();

            Assert.That(host.OutArguments.UsarValidacionAutomatica, Is.True);
        }

        [Test]
        public void TestValidacionManual()
        {
            target = new ObtenerTipoDeValidacionCtg();
            servicio = new Mock<IServicioRepositorio>();
            servicio.Setup(s => s.ValidacionCtgEsAutomatica(1)).Returns(false);
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servicio.Object);
            host.InArguments.CentroId = 1;
            host.TestActivity();

            Assert.That(host.OutArguments.UsarValidacionAutomatica, Is.False);
        }
    }
}
