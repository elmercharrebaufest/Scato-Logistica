using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EgresoSinFleteFasonesTransmisionASapAsincronicoTest
    {
        private EgresoSinFleteFasonesTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioSapAsincronico> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new EgresoSinFleteFasonesTransmisionASapAsincronico();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<IServicioSapAsincronico>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            EgresoSinFleteFazones dto = new EgresoSinFleteFazones(){Almacen = "1", CUITCliente = "1", CUITD = "1", Cantidad = "1000", Centro = "1", Material = "1", NomChofer = "Pedro" };
            
            host.InArguments.Request = new EgresoSinFleteFazonesRequest(){EgresoSinFleteFazones = dto};
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }
    }
}
