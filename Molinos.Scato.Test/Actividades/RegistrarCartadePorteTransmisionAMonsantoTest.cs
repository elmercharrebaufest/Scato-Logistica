using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RegistrarCartadePorteTransmisionAMonsantoTest
    {
        private RegistrarCartadePorteTransmisionAMonsanto target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComand;
        private Mock<WaybillManagementPODv2> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarCartadePorteTransmisionAMonsanto();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComand = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<WaybillManagementPODv2>();
            host.Extensions.Add(srvSap.Object);
            host.Extensions.Add(srvComand.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            CartaPorteTransporteAutomotorRegistro dto = new CartaPorteTransporteAutomotorRegistro() { };

            host.InArguments.Request = new ParametrosRegistro() { Item = dto };
            srvSap.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>())).Returns(new registrarCartaDePorteResponse(new RespuestaRegistro{Item = new MuestraRequerida()}));

            var resultado = host.TestActivity();

            //var a = new WaybillRegistrationFault();
            //a.webServiceError = new[] { new webServiceError { code = "80000", description = "El número de CTG 111223351 ya existe" }, new webServiceError { code = "1000", description = "La carta de porte ya se encuentra registrada." } };
            //var xml = a.ToXml();
            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }
    }
}
