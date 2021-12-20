using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EgresoSinFleteFasonesTransmisionASapTest
    {
        private EgresoSinFleteFasonesTransmisionASap target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<ZSDWS_SCATO> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new EgresoSinFleteFasonesTransmisionASap();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<ZSDWS_SCATO>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            EgresoSinFleteFazones dto = new EgresoSinFleteFazones(){Almacen = "1", CUITCliente = "1", CUITD = "1", Cantidad = "1000", Centro = "1", Material = "1", NomChofer = "Pedro" };
            
            host.InArguments.Request = new EgresoSinFleteFazonesRequest(){EgresoSinFleteFazones = dto};
            srvSap.Setup(s => s.EgresoSinFleteFazones(It.IsAny<EgresoSinFleteFazonesRequest>())).Returns(new EgresoSinFleteFazonesResponse1(){EgresoSinFleteFazonesResponse = new EgresoSinFleteFazonesResponse(){Resultado = new ZMMBALANZA6(){XBLNR = "456",MBLNR = "123", MSGNR = "AAA", TEXT = ""}}});

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }
    }
}
