using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RegistrarCartadePorteTransmisionAMonsantoAsincronicoTest
    {
        private RegistrarCartadePorteTransmisionAMonsantoAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioSapAsincronico> srvSap;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarCartadePorteTransmisionAMonsantoAsincronico();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            srvSap = new Mock<IServicioSapAsincronico>();
            host.Extensions.Add(srvSap.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            var dto = new CartaPorteTransporteAutomotorRegistro { ctg = "12341232" };

            host.InArguments.Request = new ParametrosRegistro() { Item = dto };
            host.InArguments.InstanceId = new Guid();
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;

            var resultado = host.TestActivity();

            Assert.That(host.OutArguments.FuncionaServicio, Is.True);
        }
    }
}
