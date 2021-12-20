using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class MovimientoStockTransmisionSapTest
    {
        private MovimientoStockTransmisionSap target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<ZSDWS_SCATO> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new MovimientoStockTransmisionSap();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<ZSDWS_SCATO>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestTransmitirASap()
        {
            
            host.InArguments.Request = new MovAjuste() { Almacen = "1", Cantidad = "1000", Centro = "1", Material = "1", NroDocumento = "1010", Patente = "ABC123", FechaContab = "2015-03-20" };
            srvSap.Setup(s => s.MovAjuste(It.IsAny<MovAjusteRequest>())).Returns(new MovAjusteResponse1() { MovAjusteResponse = new MovAjusteResponse() { Resultado = new ZMMBALANZA5() { MBLNR = "123", MSGNR = "000", TEXT = "" } } });

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }

        [Test]
        public void TestTransmitirASapFallo()
        {

            host.InArguments.Request = new MovAjuste() { Almacen = "1", Cantidad = "1000", Centro = "1", Material = "1", NroDocumento = "1010", Patente = "ABC123", FechaContab = "2015-03-20" };
            srvSap.Setup(s => s.MovAjuste(It.IsAny<MovAjusteRequest>())).Returns(new MovAjusteResponse1() { MovAjusteResponse = new MovAjusteResponse() { Resultado = new ZMMBALANZA5() { MBLNR = "123", MSGNR = "", TEXT = "" } } });

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.False);
        }
    }
}
