using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RegistrarMuestreoYPesajeTransmisionAMonsantoTest
    {
        private RegistrarMuestreoYPesajeTransmisionAMonsanto target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<WaybillManagementPODv2> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarMuestreoYPesajeTransmisionAMonsanto();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<WaybillManagementPODv2>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            MuestreoPesajeTransporteAutomotor dto = new MuestreoPesajeTransporteAutomotor() { };

            host.InArguments.Request = new registerSampleAndWeightRequest() { Item = dto };
            srvSap.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>())).Returns(new registrarMuestreoYPesajeResponse("OK"));

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }
    }
}
