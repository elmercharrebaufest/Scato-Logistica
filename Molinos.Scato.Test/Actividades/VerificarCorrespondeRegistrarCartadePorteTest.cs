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
    public class VerificarCorrespondeRegistrarCartadePorteTest
    {
        private VerificarCorrespondeRegistrarCartadePorte target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new VerificarCorrespondeRegistrarCartadePorte();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestVerificarCorrespondeRegistrarCartadePorte()
        {
            srvRepositorio.Setup(s => s.RequiereTecnologia(It.IsAny<Guid>())).Returns(true);

            host.InArguments.InstanceId = new Guid();
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            var resultado = host.TestActivity();

            var corresponde = resultado.First(f => f.Key == "CorrespondeLlamada").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(true));
        }

        [Test]
        public void TestVerificarCorrespondeRegistrarCartadePorteFalso()
        {
            srvRepositorio.Setup(s => s.RequiereTecnologia(It.IsAny<Guid>())).Returns(false);
            host.InArguments.InstanceId = new Guid();
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            var resultado = host.TestActivity();
            
            var corresponde = resultado.First(f => f.Key == "CorrespondeLlamada").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(corresponde, Is.EqualTo(false));
        }
    }
}
